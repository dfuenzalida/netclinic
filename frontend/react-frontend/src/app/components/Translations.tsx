// A simple translation utility

export interface Translations {
    [key: string]: string;
}

export interface MultiLangTranslations {
    [lang: string]: Translations;
}

const MultiLangTranslations: MultiLangTranslations = {
    en: {
        "welcome": "Welcome",
        "required": "is required",
        "notFound": "has not been found",
        "duplicate": "is already in use",
        "nonNumeric": "must be all numeric",
        "duplicateFormSubmission": "Duplicate form submission is not allowed",
        "typeMismatch.date": "invalid date",
        "typeMismatch.birthDate": "invalid date",
        "owner": "Owner",
        "firstName": "First Name",
        "lastName": "Last Name",
        "address": "Address",
        "city": "City",
        "telephone": "Telephone",
        "owners": "Owners",
        "addOwner": "Add Owner",
        "findOwner": "Find Owner",
        "findOwners": "Find Owners",
        "updateOwner": "Update Owner",
        "vets": "Veterinarians",
        "name": "Name",
        "specialties": "Specialties",
        "none": "none",
        "pages": "pages",
        "first": "First",
        "next": "Next",
        "previous": "Previous",
        "last": "Last",
        "somethingHappened": "Something happened...",
        "pets": "Pets",
        "home": "Home",
        "error": "Error",
        "telephone.invalid": "Telephone must be a 10-digit number",
        "layoutTitle": "PetClinic :: a Spring Framework demonstration",
        "pet": "Pet",
        "birthDate": "Birth Date",
        "type": "Type",
        "previousVisits": "Previous Visits",
        "date": "Date",
        "description": "Description",
        "new": "New",
        "addVisit": "Add Visit",
        "editPet": "Edit Pet",
        "ownerInformation": "Owner Information",
        "visitDate": "Visit Date",
        "editOwner": "Edit Owner",
        "addNewPet": "Add New Pet",
        "petsAndVisits": "Pets and Visits"
    },
    es: {
        "welcome": "Bienvenido",
        "required": "Es requerido",
        "notFound": "No ha sido encontrado",
        "duplicate": "Ya se encuentra en uso",
        "nonNumeric": "Sólo debe contener numeros",
        "duplicateFormSubmission": "No se permite el envío de formularios duplicados",
        "typeMismatch.date": "Fecha invalida",
        "typeMismatch.birthDate": "Fecha invalida",
        "owner": "Propietario",
        "firstName": "Nombre",
        "lastName": "Apellido",
        "address": "Dirección",
        "city": "Ciudad",
        "telephone": "Teléfono",
        "owners": "Propietarios",
        "addOwner": "Añadir propietario",
        "findOwner": "Buscar propietario",
        "findOwners": "Buscar propietarios",
        "updateOwner": "Actualizar propietario",
        "vets": "Veterinarios",
        "name": "Nombre",
        "specialties": "Especialidades",
        "none": "ninguno",
        "pages": "páginas",
        "first": "Primero",
        "next": "Siguiente",
        "previous": "Anterior",
        "last": "Último",
        "somethingHappened": "Algo pasó...",
        "pets": "Mascotas",
        "home": "Inicio",
        "error": "Error",
        "telephone.invalid": "El número de teléfono debe tener 10 dígitos",
        "layoutTitle": "PetClinic :: una demostración de Spring Framework",
        "pet": "Mascota",
        "birthDate": "Fecha de nacimiento",
        "type": "Tipo",
        "previousVisits": "Visitas anteriores",
        "date": "Fecha",
        "description": "Descripción",
        "new": "Nuevo",
        "addVisit": "Agregar visita",
        "editPet": "Editar mascota",
        "ownerInformation": "Información del propietario",
        "visitDate": "Fecha de visita",
        "editOwner": "Editar propietario",
        "addNewPet": "Agregar nueva mascota",
        "petsAndVisits": "Mascotas y visitas"
    },
}

export default function T(msg: string): string {
    const userLang = navigator?.language?.slice(0, 2);
    const translations = MultiLangTranslations[userLang];
    if (translations && translations[msg]) {
        return translations[msg];
    } else {
        // Fallback to English or the original message if not found
        return MultiLangTranslations['en'][msg] || msg;
    }
}