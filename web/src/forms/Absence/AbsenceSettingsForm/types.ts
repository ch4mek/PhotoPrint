import { FormikWithBackupRenderProps } from '@/forms/components/FormikWithBackup/types';
import { AgreementCorrectionValues } from '@/types/absences';

export interface AbsenceSettingsFormValues {
	amount: number;
	comment: string;
}

export interface AbsenceSettingsFormProps {
	handleClose: () => void;
	onSubmit: (
		payload: Omit<AgreementCorrectionValues, 'agreementId'>
	) => Promise<void>;
	isLoading: boolean;
	userId: number | string;
	formType: string;
	absenceName: string;
}

export type FormContentProps =
	FormikWithBackupRenderProps<AbsenceSettingsFormValues> &
		Pick<AbsenceSettingsFormProps, 'isLoading' | 'handleClose'>;
