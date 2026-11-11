import { FormikErrors, FormikState } from 'formik';
import { SetStateAction } from 'react';

import type { SignatureProps } from '../Signature/types';

import { NotificationType } from '@/constants/notifications';

export interface SignatureDialogProps {
	isDialogOpen: boolean;
	setIsDialogOpen: (value: boolean) => void;
	resetForm: (
		nextState?: Partial<FormikState<SignatureProps>> | undefined
	) => void;
	showToast: (message: string, type: NotificationType) => void;
	initValues: SignatureProps;
	validateForm: (values?: any) => Promise<FormikErrors<SignatureProps>>;
	setSubmittedValues: React.Dispatch<
		SetStateAction<SignatureProps | undefined>
	>;
}
