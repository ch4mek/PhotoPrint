import { AbsencesFormValues, AcceptPolicesItem } from '@/types/absences';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export interface AbsenceFormProps {
	isLoading: boolean;
	handleClose: VoidFunction;
	handleFormSubmit: (values: AbsencesFormValues) => Promise<void>;
	id?: number | string | null;
	checkIsNameExist: (name: string, id?: string | number) => boolean;
}

type DetailsConvertedFormValues = Omit<
	AbsencesFormValues,
	| 'type'
	| 'trackingUnitType'
	| 'activityType'
	| 'transferType'
	| 'accrualType'
	| 'isSubstitutesToApproveAllowed'
> & {
	type: SimpleConfigurableConfigurationItem | null;
	trackingUnitType: SimpleConfigurableConfigurationItem | null;
	activityType: SimpleConfigurableConfigurationItem | null;
	accrualType: SimpleConfigurableConfigurationItem | null;
	transferType: SimpleConfigurableConfigurationItem | null;
};

type ApprovalsConvertedFormValues = {
	isSubstitutesToApproveAllowed: boolean;
	acceptPolicies: AcceptPolicesItem[];
};

export interface AbsenceFormValues {
	details: DetailsConvertedFormValues;
	approvals: ApprovalsConvertedFormValues;
}
