import { FormikErrors, FormikTouched } from 'formik';

import {
	AcceptPolicesItem,
	AcceptPolicesScope,
	AcceptPolicesUser,
} from '@/types/absences';

export interface DynamicFieldProps {
	value: AcceptPolicesItem;
	roleOptionsValues: AcceptPolicesScope[];
	usersOptions: AcceptPolicesUser[];
	onChange: (value: AcceptPolicesItem, changedField?: string) => void;
	isSubstitute?: boolean;
	error: FormikErrors<AcceptPolicesItem> | undefined;
	fieldTouched?: FormikTouched<AcceptPolicesItem>;
	isShowIcon?: boolean;
	isShowUserField?: boolean;
}
