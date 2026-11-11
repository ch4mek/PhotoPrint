import type { UserFormValues } from '@/forms/types';

export interface CertificateFormProps {
	initialValues: CertificateFormValues;
	isPopOverVisible: boolean;
	togglePopOver: () => void;
}

export type CertificateFormValues = Pick<
	UserFormValues,
	// TODO: typescript-refactor - replace with enum
	'position' | 'firstName' | 'lastName' | 'department' | 'stack' | 'date'
> & {
	id: number;
};
