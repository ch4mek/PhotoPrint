import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import dayjs, { Dayjs } from 'dayjs';
import { useCallback } from 'react';

import {
	PERFORMANCE_DATE_PICKER_INNER_LABEL,
	POPUP_TITLE,
	USER_AUTOSELECT_LABEL,
} from '../../constants';
import { FieldsWrapper } from '../../styles';

import { extractKeyById } from '@/helpers/extractKey';
import { createSelector } from '@/stores/helpers';
import { usePerformanceReviewStore } from '@/stores/performanceReview/store';
import { PerformanceReviewUserData } from '@/types/performanceReview';

import { AddingPerformanceReviewFormContentProps } from './types';

export const AddingPerformanceReviewFormContent = ({
	formikProps,
	isInitialValueDisabled,
	initialValues,
	onClose,
}: AddingPerformanceReviewFormContentProps) => {
	const {
		isSubmitting,
		values,
		errors,
		touched,
		setFieldValue,
		setFieldTouched,
		submitForm,
		handleOutsideClose,
	} = formikProps;

	const { users } = usePerformanceReviewStore(createSelector('users'));

	const isUserError = !!errors.user && touched.user;
	const userErrorText = errors.user;
	const isDateErrorText = errors.date && touched.date ? errors.date : undefined;

	const handleChangeUserFullNames = useCallback(
		(value: PerformanceReviewUserData | null) => {
			setFieldValue('user', value);
			setFieldTouched('user');
		},
		[]
	);

	const handleChangeDate = useCallback((value: Dayjs | null) => {
		if (value === null) {
			setFieldValue('date', null, true);

			return;
		}

		setFieldValue('date', value, true);
		setFieldTouched('date', true);
	}, []);

	return (
		<PopUp
			title={POPUP_TITLE}
			type="form"
			onClose={onClose}
			onShadowClick={handleOutsideClose}
			showLoader={isSubmitting}
			controls={{
				positiveControl: { onClick: submitForm },
				negativeControl: { onClick: onClose },
			}}
		>
			<FieldsWrapper>
				<Autoselect
					isRequired
					valueField="fullName"
					value={values.user}
					options={users}
					label={USER_AUTOSELECT_LABEL}
					error={isUserError}
					errorText={userErrorText}
					onChange={handleChangeUserFullNames}
					disabled={Boolean(initialValues?.userData && isInitialValueDisabled)}
					getKey={extractKeyById}
				/>

				<DatePicker
					isRequired
					value={dayjs(values.date)}
					innerLabel={PERFORMANCE_DATE_PICKER_INNER_LABEL}
					size="fullWidth"
					errorText={isDateErrorText}
					onChange={handleChangeDate}
				/>
			</FieldsWrapper>
		</PopUp>
	);
};
