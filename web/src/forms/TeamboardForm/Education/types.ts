import { PersistentConfigurationItem } from '@/types/configuration';
import { EducationsResponse } from '@/types/educations';

export type EducationFormValues = {
	educationLevel: PersistentConfigurationItem | null;
	educationalInstitution: string;
	faculty: string;
	specialization: string;
	studyForm: PersistentConfigurationItem | null;
	graduationYear: string;
	comment: string;
};

export type EducationFormProps = {
	education: EducationsResponse | null;
	closePopup: VoidFunction;
};
