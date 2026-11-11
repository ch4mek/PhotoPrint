import { AbsenceAssignmentValues } from '@/types/absences';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export interface AbsenceAssignmentFormProps {
	handleClose: VoidFunction;
	isLoading: boolean;
	handleFormSubmit: (data: AbsenceAssignmentValues) => Promise<void>;
}

export interface TransferListItem {
	id: number;
	title: string;
}

export interface AbsenceAssignmentFormValues {
	absence: SimpleConfigurableConfigurationItem<number> | null;
	agreement: SimpleConfigurableConfigurationItem<number> | null;
	effectiveFromDate: Date | null;
}
