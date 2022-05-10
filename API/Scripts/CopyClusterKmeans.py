# -*- coding: utf-8 -*-
"""
Created on Tue Nov 23 10:38:16 2021

@author: Admin
"""
#para calculos cientificos
from ast import parse
from pandas.core import indexing
import numpy as np 
#para el analisis de datos
import pandas as pd 
import json
import sys
# para importacion del metodo
from sklearn.cluster import KMeans 

from sklearn.decomposition import PCA

def Clustering(numClusters):

    df=pd.read_csv(r"C:\Users\Admin\Documents\ProyectoRegresionesReact\Learning_Analytics.API\API\Scripts\input\student-mat.csv", engine='python')
    # df=pd.read_csv('./input/student-mat.csv', engine='python')
    #df.info() #vemos que es lo que contiene el objeto datos
    #df.head() #vemos las filas de los datos
    #linea que se usa para eliminar o no tomar en cuenta un elemento o columna
    #df_variables=df.drop(['school'], axis=1) 
    df_variables=df.drop(['G1', 'G2', 'G3', 'address', 'Pstatus', 'reason', 'famsup', 'school', 'nursery', 'goout'], axis=1)
    #Aqui podremos observar todo los estatisticos maximos, minimos, cuartiles, primedio
    #desviasion estandar, etc.
    #df_variables.describe()


    #Utilizamos el mismo metodo que se utiliza en el data set para convertir
    #todo a numeros enteros.


    # 0 stands for F and 1 stands for M. [F=Femenino, M=Masculino]
    # Here we will convert all the binary columns to integers.
    df_variables['b_sex'] = df_variables['sex'].apply(lambda x: 0 if x == 'F' else 1)
    df_variables['b_sex'].value_counts()

    # Interestingly there are more students in families that are greater than 3.
    # Could it be possible that all family members are in the same school? This might be a reason why it is higher.
    # LE3 = Less than 3. [0], GE3 = Greater than 3.[1]
    df_variables['b_famsize'] = df_variables['famsize'].apply(lambda x: 0 if x == 'LE3' else 1)
    df_variables['b_famsize'].value_counts()

    # 0 = no and 1 = yes
    # This is an interesting column when it comes to having a positive effect on G3.
    df_variables['b_paidxtraclasses'] = df_variables['paid'].apply(lambda x: 0 if x == 'no' else 1)
    df_variables['b_paidxtraclasses'].value_counts()

    # 0 = no and 1 = yes
    df_variables['b_xtraactivities'] = df_variables['activities'].apply(lambda x: 0 if x == 'no' else 1)
    df_variables['b_xtraactivities'].value_counts()

    # 0 = no and 1 = yes
    # It has a high correlation however, we only have 20 students that are not interested in having a high education and 
    # thus this column should not be taken into consideration.
    df_variables['b_higher_education'] = df_variables['higher'].apply(lambda x: 0 if x == 'no' else 1)
    df_variables['b_higher_education'].value_counts()


    # continue with the analisis.
    df_variables['b_internet'] = df_variables['internet'].apply(lambda x: 0 if x == 'no' else 1)
    df_variables['b_internet'].value_counts()

    # Interestingly when people are not in a romantic relationship they tend to get better grades.
    df_variables['b_romantic'] = df_variables['romantic'].apply(lambda x: 0 if x == 'no' else 1)
    df_variables['b_romantic'].value_counts()


    df_variables['b_guardian'] = df_variables['guardian'].apply(lambda x: 0 if x == 'mother' else (1 if x=='father' else 2))
    df_variables['b_guardian'].value_counts()

    df_variables['b_schoolsup'] = df_variables['schoolsup'].apply(lambda x: 0 if x == 'no' else 1)
    df_variables['b_schoolsup'].value_counts()

    #variable Mjob
    df_variables['b_Mjob'] = df_variables['Mjob'].apply(lambda x: 0 if x == 'nominal' else (1 if x=='health' else (2 if x=='services' else (3 if x=='at home' else 4))))
    df_variables['b_Mjob'].value_counts()

    #variable Fjob
    df_variables['b_Fjob'] = df_variables['Fjob'].apply(lambda x: 0 if x == 'nominal' else (1 if x=='health' else (2 if x=='services' else (3 if x=='at home' else 4))))
    df_variables['b_Fjob'].value_counts()

    df_variables_new=df_variables.drop(columns=['sex','famsize','paid','activities','higher','internet',
                        'romantic','guardian','schoolsup','Mjob','Fjob']) 
    #df_variables_new.info()


    #normalizamos los valores para que se pongan entre los mismo rangos
    #ya que los valores estan muy distintos
    df_norm=(df_variables_new-df_variables_new.min())/(df_variables_new.max()-df_variables_new.min())
    df_norm

    #df_norm.describe()

    #implementaremos el metdo codo de jambu
    #crea difef tipos de clustering para ver que tan simirales son los vecinos 
    #e irlos mostrando o plasmandolos dentro de una grafica.

    #wcss es la suma de los cuadrados de cada grupo


    arreglowcss = []#variable para almacenar

    for i in range (1,11):#loop para crear agrupaciones se pone hasta cual numero quieres +1
        kmeans = KMeans(n_clusters = i, max_iter=300)
        kmeans.fit(df_norm) #aplicamos K/means a la base de datos
        arreglowcss.append(kmeans.inertia_)
        
        
    # plt.plot(range(1,11), arreglowcss)
    # plt.title('codo de Jambu')
    # plt.xlabel('Numero de clusters')
    # plt.ylabel('WCSS')#indicador de que tan similares son los individuos dentro de los clusters
    # plt.show()

    #aplicamos el metodo Kmeans a la BD

    clustering = KMeans(n_clusters = numClusters, max_iter = 300)#creamos el modelo
    clustering.fit(df_norm)#aplicamos el modelo a la BD

    #agramos la clasificacion al archivo original

    df['KMeans_Clusters'] = clustering.labels_ #los resultados se guardan en label_ dentro del modelo
    #df.head()

    #visualizacion de los clustering que se fomaron
    #utilizando graficos con analisis de componentes principales PCA


    pca = PCA(n_components=2)#modelo de 2 dimensiones 
    pca_df = pca.fit_transform(df_norm)#obtenemos los dos componentes principales
    pca_df_data = pd.DataFrame(data = pca_df, columns = ['Componente_1', 'Componente_2']) #Creamos dataframe que contega los elementos principales
    pca_nombres_df = pd.concat([pca_df_data, df[['KMeans_Clusters']]], axis=1) #agegamos la columna del clustering

    pca_nombres_df
    #Separar data frame entre el numero de cluster
    SplittedJson = SplitIntoArray(numClusters, pca_nombres_df)

    results = pca_nombres_df.to_json(orient="records")
    parsed = json.loads(results) 
    json_parsed = json.dumps(parsed, separators=(',',':'))

    # print('{ "employees" : [{ "firstName":"John" , "lastName":"Doe" },{ "firstName":"Anna" , "lastName":"Smith" },{ "firstName":"Peter" , "lastName":"Jones" } ]}')
    print(SplittedJson)
    # with open("C:\Dev\Learning_Analytics.API\API\Scripts\output", "w") as outfile:
    #     outfile.write(json_parsed)


def SplitIntoArray(numClusters, dataFrame):
    data = {}
    for number in range(numClusters):
        res = dataFrame[dataFrame.KMeans_Clusters == number].to_json(orient="records")
        arrayName = 'Array_' + str(number)
        data[arrayName] = []
        
        parsed = json.loads(res)
        json_dumped = json.dumps(parsed, separators=(',',':'))
        #data[arrayName].append(json_dumped[1:(len(json_dumped)-2)])##(json_dumped)
        data[arrayName] = parsed
        
    json_revamped = json.dumps(data, separators=(',',':'))
        
        
        # result = dfjson.to_json(orient="records")
        # parsed = json.loads(result)
        # if not parsed == "":
            # json_dumped += json.dumps(parsed, indent=4)
        # else:
            # json_dumped = json.dumps(parsed, indent=4)
    return json_revamped

if len(sys.argv) == 2:
    num_clusters = int(sys.argv[1])
    if num_clusters > 0:
        Clustering(num_clusters)
else:
    print("Favor de pasar como argumento el numero de clusters")



    # pca_nombres_df.to_csv('C:/Users/Admin/Documents/PrediccionDeCalificcionesSecundaria/clusters_creados/MetodoPCA1.csv')

    # #por ultimo guardamos los clusters dentro de nuestro disco duro
    # df.to_csv('C:/Users/Admin/Documents/PrediccionDeCalificcionesSecundaria/clusters_creados/Archivo5,1.csv')



