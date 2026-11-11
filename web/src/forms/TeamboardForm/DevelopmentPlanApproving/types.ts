export enum DevelopmentPlanApprovingFormField {
	StartDate = 'startDate',
	ConfirmationDate = 'confirmationDate',
}

export interface DevelopmentPlanApprovingFormValues {
	[DevelopmentPlanApprovingFormField.StartDate]: Date;
	[DevelopmentPlanApprovingFormField.ConfirmationDate]: Date | null;
}

export interface DevelopmentPlanApprovingFormProps {
	startDate: string;
	handleClose: VoidFunction;
	onSubmit: (confirmationDate: DevelopmentPlanApprovingFormValues) => void;
}
