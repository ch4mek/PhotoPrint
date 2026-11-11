import { FormikErrors, FormikTouched } from 'formik';

import { AcceptPolicesItem } from '@/types/absences';

export type ApprovalsFormValuesProps = {
	isSubstitutesToApproveAllowed: boolean;
	acceptPolicies: AcceptPolicesItem[];
};

export interface ApprovalsTabProps {
	values: ApprovalsFormValuesProps;
	errors?: FormikErrors<ApprovalsFormValuesProps>;
	touched?: FormikTouched<ApprovalsFormValuesProps>;
	setFieldValue: (field: string, value: unknown) => void;
}
