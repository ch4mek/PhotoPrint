import { FormikErrors, FormikTouched } from 'formik';

import { AbsenceDetailsFields, AbsencesFormValues } from '@/types/absences';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export interface DetailsTabProps {
	values: ConvertedFormValues;
	errors?: FormikErrors<ConvertedFormValues>;
	touched?: FormikTouched<ConvertedFormValues>;
	setFieldValue: (field: string, value: unknown) => void;
	setFieldTouched: (field: string, touched: boolean) => void;
	validateField: (field: string) => void;
	absenceTypesOptions: SimpleConfigurableConfigurationItem[];
	activityTypesOptions: SimpleConfigurableConfigurationItem[];
	accrualTypeOptions: SimpleConfigurableConfigurationItem[];
	disableAutoselectFields: boolean;
}

interface AutoSelectFieldDataItem {
	name: AbsenceDetailsFields;
	options: Array<SimpleConfigurableConfigurationItem>;
	withWrapper: boolean;
}

export interface AutoSelectFieldData {
	firstPartFields: AutoSelectFieldDataItem[];
	secondPartFields: AutoSelectFieldDataItem[];
}

type ConvertedFormValues = Omit<
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
