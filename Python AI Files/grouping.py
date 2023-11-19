#!/usr/bin/env python
# coding: utf-8

# In[1]:


import nltk
import gensim
import gensim.corpora as corpora
import pandas as pd


# In[2]:


# define the corpus
data = pd.read_excel('data/contexts.xlsx')
corpus = data['contexts']


# In[3]:


import pandas as pd
from sklearn.feature_extraction.text import CountVectorizer

countvec = CountVectorizer()
countvecfit = countvec.fit_transform(corpus)
bagofwords = pd.DataFrame(countvecfit.toarray(),columns=countvec.get_feature_names_out())


# In[4]:


# get the corpus tokenized
corpusTokens = []
for text in corpus:
  corpusTokens.append(nltk.word_tokenize(text))
dictionary = corpora.Dictionary(corpusTokens)


# In[5]:


indeces = []
for word in dictionary:
    index =  [i for i, x in enumerate(bagofwords[dictionary[word]]) if x == 1] 
    indeces.append(index)


# In[6]:


groups=[]
for index in indeces:
    if len(index)>1 and len(index)<6:
        group = index
        groups.append(group)    


# In[7]:


lengroups = len(groups)
for i in range(lengroups-1):
    j=i
    while j < len(groups)-1:
        j=j+1
        if len(set(groups[i])&set(groups[j]))>0:
            groups[i]=list(set(groups[i]+groups[j]))
            groups.remove(groups[j])
            j=i 


# In[8]:


clusters=[]
for i in range(len(corpus)):
    clusters.append('')


# In[9]:


current=1
for g in groups:
    for i in range(len(g)):
        clusters[g[i]]=current
    current = current + 1 


# In[10]:


data['Clusters']=clusters
data.to_excel('data/sensegroups.xlsx',index=False)
print('Clustering to groups based on sense graphs is finished!')
print('Check clustering in the excel file at Data\\sensegroups.xlsx')


# In[11]:


#LDA
# create bag of words
corpus_bow = [dictionary.doc2bow(doc) for doc in corpusTokens]
# Make an index to word dictionary.
temp = dictionary[0]  # This is only to "load" the dictionary.
id2word = dictionary.id2token

ldaModel = gensim.models.LdaModel(corpus=corpus_bow,id2word=id2word,chunksize=2000,iterations=400,passes=20,num_topics=5)


# In[12]:


scores = ldaModel.get_document_topics(corpus_bow[0])
max=0
ind=0
for s in scores:
    if s[1]>max:
        max=s[1]
        ind=s[0]


# In[13]:


groups=[]
for doc in corpus_bow:
    scores=ldaModel.get_document_topics(doc)
    max=0
    ind=0
    for s in scores:
        if s[1]>max:
            max=s[1]
            ind=s[0]
    ind=ind+1        
    groups.append(ind)     


# In[14]:


data['LDA']=groups
data.to_excel('data/LDAgroups.xlsx',index=False)


# In[ ]:


print("Program finished successfully!!")
print('Check groups based on LDA method with num of groups = 5 in the excel file at data\LDAgroups.xlsx')

