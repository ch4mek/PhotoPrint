export interface DevelopmentPlanFormProps {
	handleClose: VoidFunction;
	onSubmit: (values: DevelopmentPlanFormValues) => void;
	getDevPlanById?: (idOfDevPlan: number) => DevelopmentPlanFormValues;
	id?: number;
}

export interface DevelopmentPlanFormValues {
	startDate: Date | null;
	deadlineDate: Date | null;
	summary: string;
	notes: string;
}

export enum DevelopmentPlanFormField {
	StartDate = 'startDate',
	DeadlineDate = 'deadlineDate',
	Summary = 'summary',
	Notes = 'notes',
}
