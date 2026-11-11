import { useTheme } from '@emotion/react';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useMemo } from 'react';

import {
	internshipFinalResultOptions,
	internshipFormFields,
	internshipTypeOptions,
	POPUP_TITLE,
} from '../../constants';
import { useInternshipForm } from '../../hooks/useInternshipForm';
import { FormRow } from '../../styles';
import { InternshipFormFields } from '../../types';
import { getPopUpStylesConfig } from '../../utils/getPopUpStylesConfig';
import { TechnologiesFields } from '../TechnologiesFields';

import { TextField } from '@/fields/Text';
import { FormWrapper } from '@/forms/styles';
import { useResize } from '@/hooks/useResize';
import dayjs from '@/lib/dayjs';

import { InternshipFormContentProps } from './types';

export function InternshipFormContent({
	onClose,
	handleOutsideClose,
}: InternshipFormContentProps) {
	const theme = useTheme();
	const [, windowHeight] = useResize();
	const {
		values,
		errors,
		touched,
		isSubmitting,
		submitForm,
		setFieldValue,
		handleFieldChange,
		handleDateChange,
		handleNumberChange,
	} = useInternshipForm();

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	return (
		<PopUp
			title={POPUP_TITLE}
			onClose={onClose}
			onShadowClick={handleOutsideClose}
			controls={{
				positiveControl: { onClick: submitForm },
				negativeControl: { onClick: onClose },
			}}
			type="form"
			showLoader={isSubmitting}
			stylesConfig={popUpStylesConfig}
		>
			<FormWrapper>
				<TextField
					{...internshipFormFields.internshipSpecialization}
					onChange={setFieldValue}
					value={values.internshipSpecialization}
					isRequired
				/>
				<FormRow>
					<Autoselect
						label={internshipFormFields.internshipType.label}
						value={values.internshipType}
						options={internshipTypeOptions}
						onChange={handleFieldChange(InternshipFormFields.Type)}
						error={Boolean(errors.internshipType && touched.internshipType)}
						errorText={errors.internshipType}
						isRequired
					/>
					<TextField
						{...internshipFormFields.internshipEngineerStage}
						onChange={handleNumberChange}
						value={values.internshipEngineerStage}
						isRequired
					/>
				</FormRow>
				<FormRow columns={3}>
					<TextField
						{...internshipFormFields.internshipCompletedPoints}
						onChange={handleNumberChange}
						value={values.internshipCompletedPoints}
					/>
					<TextField
						{...internshipFormFields.internshipApprovedPoints}
						onChange={handleNumberChange}
						value={values.internshipApprovedPoints}
					/>
					<Autoselect
						label={internshipFormFields.internshipFinalResult.label}
						value={values.internshipFinalResult}
						options={internshipFinalResultOptions}
						onChange={handleFieldChange(InternshipFormFields.FinalResult)}
						error={Boolean(
							errors.internshipFinalResult && touched.internshipFinalResult
						)}
						errorText={errors.internshipFinalResult}
					/>
				</FormRow>
				<FormRow>
					<DatePicker
						innerLabel={internshipFormFields.internshipStartDate.label}
						size="fullWidth"
						errorText={
							touched.internshipStartDate ? errors.internshipStartDate : ''
						}
						value={dayjs(values.internshipStartDate)}
						onChange={handleDateChange(InternshipFormFields.StartDate)}
						isRequired
					/>
					<DatePicker
						innerLabel={internshipFormFields.internshipPlannedEndDate.label}
						size="fullWidth"
						errorText={
							touched.internshipPlannedEndDate
								? errors.internshipPlannedEndDate
								: ''
						}
						value={dayjs(values.internshipPlannedEndDate)}
						onChange={handleDateChange(InternshipFormFields.PlannedEndDate)}
						isRequired
					/>
				</FormRow>
				<TechnologiesFields />
			</FormWrapper>
		</PopUp>
	);
}
