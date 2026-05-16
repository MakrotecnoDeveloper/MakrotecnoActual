---README 
estructura del proyecto

Backend: .net framework 8.0
Frontend: Razor, Boostrap
BD: SQL Server

----------------------
Update
Create a new branch for the update, and then merge it back to the main branch after testing.
1. Create a new branch for the update:
   - Open your terminal and navigate to your project directory.
   - Run the command: `git checkout -b update-branch`

   2. Make the necessary changes and updates to your code in the new branch.
   3. After making the changes, commit them to the new branch:
   - Run the command: `git add .` to stage all the changes.
   - Run the command: `git commit -m "Update: [brief description of changes]"` to commit the changes.
   4. Once you have tested the changes and are satisfied with them, merge the update branch back to the main branch:
   - Switch to the main branch: `git checkout main`
   - Merge the update branch: `git merge update-branch`
   5. Finally, push the changes to the remote repository:
   - Run the command: `git push origin main`


   this are some instruccions to update the code and merge it back to the main branch. 
   Make sure to test the changes thoroughly before merging to avoid any issues in the main branch.


   ----------------------------------

Estructura del proyecto

se estructurara por medio de capas el proyecto de la siguiente manera:
-Capa de Repositorio: Esta capa se encargará de la comunicación con la base de datos, realizando operaciones CRUD (Crear, Leer, Actualizar, Eliminar) sobre los datos.
-Capa de Servicios: Esta capa se encargará de la lógica de negocio, procesando los datos obtenidos de la capa de repositorio y aplicando las reglas de negocio necesarias.
-Capa de Modelos: Esta capa se encargará de definir las estructuras de datos utilizadas en el proyecto, como clases o estructuras que representen los objetos del dominio.
-Controladores: Esta capa se encargará de manejar las solicitudes y respuestas, actuando como intermediario entre la capa de servicios y la capa de presentación (si es necesario).
-Capa de Helpers: Esta capa se encargará de proporcionar funciones auxiliares o utilidades que puedan ser utilizadas en diferentes partes del proyecto, como funciones de validación, formateo de datos, etc.
Esta estructura en capas ayudará a mantener el código organizado, modular y fácil de mantener, permitiendo una separación clara de responsabilidades entre las diferentes partes del proyecto.



