// Owner Types

export interface VisitDetails {
  id: number;
  visitDate: string;
  description: string;
}

export interface PetDetails {
  id: number;
  name: string;
  type: string;
  birthDate: string;
  visits: VisitDetails[];
}

export interface OwnerDetails {
  id: number;
  firstName: string;
  lastName: string;
  address: string;
  city: string;
  telephone: string;
  pets: PetDetails[];
}

// Vets

export interface Vet {
  id: number;
  firstName: string;
  lastName: string;
  specialties: Specialty[];
}

export interface Specialty {
  id: number;
  name: string;
}

export interface PetType {
  id: number;
  name: string;
}

// Hash Props, used to pass state around using the hash

export interface HashProps {
  hash: string;
  setHash: (hash: string) => void;
}


// Pagination

export interface PaginationProps {
  currentPage: number;
  setCurrentPage: (page: number) => void;
  totalPages: number;
  linkBase: string;
}

// Owners

// OwnerCreateForm

export interface OwnerCreateErrors {
  firstName?: string;
  lastName?: string;
  address?: string;
  city?: string;
  telephone?: string;
}
