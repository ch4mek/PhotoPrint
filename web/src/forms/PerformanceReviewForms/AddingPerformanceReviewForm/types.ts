import { PerformanceReviewUserData } from '@/types/performanceReview';

export interface InitialPerformanceReviewValues {
	userData?: PerformanceReviewUserData | null;
}

export interface AddingPerformanceReviewFormProps {
	onClose: VoidFunction;
	initialValues?: InitialPerformanceReviewValues;
	refetch?: VoidFunction;
	isInitialValueDisabled?: boolean;
}

export interface FormValues {
	user: PerformanceReviewUserData | null;
	date: Date | null;
}
