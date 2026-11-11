import { DevelopmentPlanFormValues } from '../../types';

import { FormikWithBackupRenderProps } from '@/forms/components/FormikWithBackup/types';

export interface DevelopmentPlanFormContentProps {
	formikProps: FormikWithBackupRenderProps<DevelopmentPlanFormValues>;
	isLoading: boolean;
	isEditMode: boolean;
	onClose: VoidFunction;
}
