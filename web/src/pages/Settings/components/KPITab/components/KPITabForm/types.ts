import { FormikProps } from 'formik';

import { KPIColorsFormValues } from '../../types';

export interface KPITabFormProps {
	formikProps: FormikProps<KPIColorsFormValues>;
	isFetching: boolean;
	isError: boolean;
	isUpdatingKPIColors: boolean;
}
