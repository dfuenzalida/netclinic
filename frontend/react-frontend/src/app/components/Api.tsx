import { OwnerDetails, PetDetails, PetType, PetVisit, VisitDetails } from "../../types/types";

// Fetch an owner by ID
export const fetchOwnerById = async (ownerId: number): Promise<OwnerDetails> => {
    try {
        const response = await fetch(`/api/owners/${ownerId}`);

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const owner = await response.json();
        return owner;
    } catch (err) {
        console.error(`Error fetching owner with ID ${ownerId}:`, err);
        throw err;
    }
};

// Fetch pets for a given owner
export const fetchPetsForOwner = async (ownerId: number): Promise<PetDetails[]> => {
    try {
        const response = await fetch(`/api/owners/${ownerId}/pets`);

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const pets = await response.json();
        return pets;
    } catch (err) {
        console.warn(`Error fetching pets for owner ${ownerId}:`, err);
        return []; // Return empty array if pets fetch fails
    }
};

// Fetch visits for a given pet
export const fetchVisitsForPet = async (ownerId: number, petId: number): Promise<VisitDetails[]> => {
    try {
        const response = await fetch(`/api/owners/${ownerId}/pets/${petId}/visits`);

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const visits = await response.json();
        return visits;
    } catch (err) {
        console.warn(`Error fetching visits for pet ${petId} of owner ${ownerId}:`, err);
        return []; // Return empty array if visits fetch fails
    }
};

export const fetchPetTypes = async (): Promise<PetType[]> => {
    try {
        const response = await fetch('/api/pet/types');

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const types = await response.json();
        return types as PetType[];
    } catch (err) {
        console.error('Error fetching pet types:', err);
        return []; // Return empty array if fetch fails
    }
}

export const fetchPetById = async (ownerId: number, petId: number): Promise<PetDetails> => {
    try {
        const response = await fetch(`/api/owners/${ownerId}/pets/${petId}`);

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const pet = await response.json();
        return pet;
    } catch (err) {
        console.error(`Error fetching pet with ID ${petId} for owner ${ownerId}:`, err);
        throw err;
    }
}

export const fetchPetVisitsById = async (ownerId: number, petId: number): Promise<PetVisit[]> => {
    try {
        const response = await fetch(`/api/owners/${ownerId}/pets/${petId}/visits`);

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const visits = await response.json();
        return visits;
    } catch (err) {
        console.error(`Error fetching pet visits for pet ${petId} of owner ${ownerId}:`, err);
        throw err;
    }
}