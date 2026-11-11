import { FormikValues } from 'formik';

type SignatureLinkType = 'phone' | 'website' | 'default';
export interface SignatureFormValues {
	fullName: string;
	position: string;
	email: string;
	phone: string;
	website: string;
}
export interface SignatureProps extends FormikValues, SignatureFormValues {
	mode?: 'default' | 'email';
}

export interface SignatureLinkProps {
	value: string;
	type?: SignatureLinkType;
}
