import { FormikValues } from 'formik';

import { SignatureProps } from '../Signature/types';

import { NotificationType } from '@/constants/notifications';

export interface SignatureControlsProps extends FormikValues {
	isValid: boolean;
	values: SignatureProps;
	setIsDialogOpen: (value: boolean) => void;
	showToast: (message: string, type: NotificationType) => void;
	showForm: () => void;
}
