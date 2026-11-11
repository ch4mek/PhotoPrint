import { useTheme } from '@emotion/react';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useCallback, useMemo } from 'react';

import { developmentPlanFormFields } from '../../constants';
import { DatesWrapper, DevelopmentPlanFormWrapper } from '../../styles';
import { DevelopmentPlanFormField } from '../../types';
import { getPopUpStylesConfig } from '../../utils';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import { DEVELOPMENT_PLAN_TITLE } from '@/constants/developmentPlan';
import { TextArea } from '@/fields/TextArea';
import { useResize } from '@/hooks/useResize';
import dayjs, { Dayjs } from '@/lib/dayjs';

import { DevelopmentPlanFormContentProps } from './types';

export const DevelopmentPlanFormContent = ({
	formikProps,
	isEditMode,
	isLoading,
	onClose,
}: DevelopmentPlanFormContentProps) => {
	const {
		setFieldValue,
		setFieldTouched,
		submitForm,
		isSubmitting,
		errors,
		touched,
		values,
		handleOutsideClose,
	} = formikProps;

	const { startDate, deadlineDate } = values;

	const theme = useTheme();
	const [, windowHeight] = useResize();

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	const handleDateChange = useCallback(
		(fieldName: string) => (value: Dayjs | null) => {
			if (value === null) {
				setFieldValue(fieldName, null, true);

				return;
			}

			setFieldValue(fieldName, value, true);
			setFieldTouched(fieldName, true);
		},
		[]
	);

	const handleTextFieldChange = useCallback(
		(fieldName: string, value: string) => {
			setFieldValue(fieldName, value, true);
			setFieldTouched(fieldName, true);
		},
		[]
	);

	return (
		<PopUp
			title={
				isEditMode
					? getEditingPopUpTitle(DEVELOPMENT_PLAN_TITLE)
					: getAddingPopUpTitle(DEVELOPMENT_PLAN_TITLE)
			}
			onClose={onClose}
			onShadowClick={handleOutsideClose}
			controls={{
				positiveControl: { onClick: submitForm },
				negativeControl: { onClick: onClose },
			}}
			type="form"
			showLoader={isLoading || isSubmitting}
			stylesConfig={popUpStylesConfig}
		>
			<DevelopmentPlanFormWrapper>
				<DatesWrapper>
					<DatePicker
						{...developmentPlanFormFields.startDate}
						errorText={
							touched.startDate && errors.startDate
								? String(errors.startDate)
								: ''
						}
						disabled={isEditMode}
						value={dayjs(startDate)}
						onChange={handleDateChange(DevelopmentPlanFormField.StartDate)}
						isRequired
					/>
					<DatePicker
						{...developmentPlanFormFields.deadlineDate}
						errorText={
							touched.deadlineDate && errors.deadlineDate
								? String(errors.deadlineDate)
								: ''
						}
						value={dayjs(deadlineDate)}
						onChange={handleDateChange(DevelopmentPlanFormField.DeadlineDate)}
					/>
				</DatesWrapper>
				<TextArea
					{...developmentPlanFormFields.summary}
					size="fullWidth"
					onChange={handleTextFieldChange}
					isRequired
				/>
				<TextArea {...developmentPlanFormFields.notes} size="fullWidth" />
			</DevelopmentPlanFormWrapper>
		</PopUp>
	);
};
