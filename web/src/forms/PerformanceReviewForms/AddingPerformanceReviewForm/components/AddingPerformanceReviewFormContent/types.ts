import { InitialPerformanceReviewValues } from '../../types';

import { FormikWithBackupRenderProps } from '@/forms/components/FormikWithBackup/types';
import { PerformanceReviewUserData } from '@/types/performanceReview';

export interface AddingPerformanceReviewFormContentProps {
	formikProps: FormikWithBackupRenderProps<{
		user: PerformanceReviewUserData | null;
		date: Date | null;
	}>;
	isInitialValueDisabled: boolean;
	onClose: VoidFunction;
	initialValues?: InitialPerformanceReviewValues;
}
