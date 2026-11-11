export interface DevelopmentPlanDeleteConfirmationProps {
	handleClose: VoidFunction;
	selectedDate: string;
	onSubmit: (id: number) => void;
	isLoading: boolean;
	id?: number;
}
