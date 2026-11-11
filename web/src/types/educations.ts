export type EducationsGridColumns =
	| 'educationLevel'
	| 'educationalInstitution'
	| 'faculty'
	| 'specialization'
	| 'studyForm'
	| 'graduationYear'
	| 'comment';

export type EducationsGridFilters = {
	educationLevel: string[];
	educationalInstitution: string;
	faculty: string;
	specialization: string;
	studyForm: string[];
	graduationYear: string;
	comment?: string;
};

export type EducationsGridFilterSetters = {
	educationLevel: (educationLevel: string[]) => void;
	educationalInstitution: (educationalInstitution: string) => void;
	faculty: (faculty: string) => void;
	specialization: (specialization: string) => void;
	studyForm: (studyForm: string[]) => void;
	graduationYear: (graduationYear: string) => void;
	comment?: (comment: string) => void;
	set: (filters: EducationsGridFilters) => void;
	clear: () => void;
};

export enum EducationsActionKeys {
	educationLevel = 'educationLevel',
	educationalInstitution = 'educationalInstitution',
	faculty = 'faculty',
	specialization = 'specialization',
	studyForm = 'studyForm',
	graduationYear = 'graduationYear',
	comment = 'comment',
	set = 'set',
	clear = 'clear',
}

export type EducationsAction = {
	type: EducationsActionKeys;
	payload?: string | string[] | EducationsGridFilters;
};

export type EducationsResponse = {
	id: number;
	educationLevel: string;
	educationalInstitution: string;
	faculty: string;
	specialization: string;
	studyForm: string;
	graduationYear: number;
	comment?: string | null;
};
