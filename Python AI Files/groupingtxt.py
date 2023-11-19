#!/usr/bin/env python
# coding: utf-8

# In[1]:


import nltk
import gensim
import gensim.corpora as corpora
import pandas as pd


# In[2]:


# define the corpus
corpus=[]
f=open('data/contexts.txt',encoding='utf-8')  
c='1'
while(c!=''):
    c = f.readline()
    if(c!=''):
        c=c.rstrip()
        corpus.append(c)
corpus[0] = corpus[0].replace('\ufeff','')    
corpus


# In[3]:


from sklearn.feature_extraction.text import CountVectorizer

countvec = CountVectorizer()
countvecfit = countvec.fit_transform(corpus)
bagofwords = pd.DataFrame(countvecfit.toarray(),columns=countvec.get_feature_names_out())
bagofwords


# In[4]:


# get the corpus tokenized
corpusTokens = []
for text in corpus:
  corpusTokens.append(nltk.word_tokenize(text))
dictionary = corpora.Dictionary(corpusTokens)
corpusTokens


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


textlineout = []
for i in range(len(corpus)):
    textline = corpus[i]+','+str(clusters[i])
    textline=textline+'\n'
    textlineout.append(textline)
print(textlineout[0])
f2 = open('Data\\egogroups.txt','w',encoding='utf-8')
f2.writelines(textlineout)
f2.close()
print('Clustering to groups based on ego graphs is finished!')
print('Check clustering in the file at Data\\egogroups.txt')
print(corpus[0]+','+str(2))


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


textlineout = []
for i in range(len(corpus)):
    textline = corpus[i]+','+str(groups[i])
    textline=textline+'\n'
    textlineout.append(textline)

f2 = open('data\\LDAgroups.txt','w',encoding='utf-8')
f2.writelines(textlineout)
f2.close()


# In[15]:


print("Program finished successfully!!")
print('Check groups based on LDA method with num of groups = 5 in the file at data\\LDAgroups.txt')

