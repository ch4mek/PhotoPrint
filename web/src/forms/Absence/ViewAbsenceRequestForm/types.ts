export interface ViewAbsenceRequestFormProps {
	id: number | string;
	handleClose: VoidFunction;
}

export interface AbsenceRequestChangesContextValues {
	hasUnsavedChanges: boolean;
	setHasUnsavedChanges: (hasChanges: boolean) => void;
}
