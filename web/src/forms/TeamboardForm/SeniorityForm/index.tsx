import { useTheme } from '@emotion/react';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { FormikHelpers } from 'formik';
import { useCallback, useMemo } from 'react';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import { currentDate } from '@/constants/date';
import { seniorityEntityName } from '@/constants/formEntities';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormWrapper } from '@/forms/styles';
import { getSpecializationPopupStyles } from '@/forms/TeamboardForm/constants';
import { SkillsList } from '@/forms/TeamboardForm/SeniorityForm/components/SkillsList';
import {
	initialSkillsValues,
	FORM_NAME,
} from '@/forms/TeamboardForm/SeniorityForm/constants';
import { skillsValidationSchema } from '@/forms/TeamboardForm/SeniorityForm/schema';
import {
	SeniorityFormProps,
	SeniorityFormsValues,
	SkillsTouched,
} from '@/forms/TeamboardForm/SeniorityForm/types';
import { convertResponseToFormData } from '@/forms/TeamboardForm/SeniorityForm/utils/convertResponseToFormData';
import { extractSkillsErrors } from '@/forms/TeamboardForm/SeniorityForm/utils/extractSkillsErrors';
import { useResize } from '@/hooks/useResize';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { useGetAssignedUsersSoftSkills } from '@/queries/softSkills/useGetAssignedUsersSoftSkills';
import { SeniorityLevelType } from '@/types/configuration';

export function SeniorityForm({
	userId,
	handleSubmit,
	snapshotDate,
	handleClose,
	editSnapshotId,
}: SeniorityFormProps) {
	const { data, isLoading, isFetching } = useGetAssignedUsersSoftSkills(
		userId,
		editSnapshotId
	);
	const [, windowHeight] = useResize();
	const theme = useTheme();

	const formSkillsInfo = useMemo(() => convertResponseToFormData(data), [data]);

	const initialValues: SeniorityFormsValues = useMemo(() => {
		if (formSkillsInfo) {
			const date =
				editSnapshotId && snapshotDate ? dayjs(snapshotDate) : currentDate;

			return {
				date: dayjs(date).startOf('day'),
				skills: formSkillsInfo,
			};
		}

		return initialSkillsValues;
	}, [formSkillsInfo, snapshotDate, editSnapshotId]);

	const popUpStylesConfig = useMemo(
		() => getSpecializationPopupStyles(theme, windowHeight),
		[theme, windowHeight]
	);

	const handleFormSubmit = useCallback(
		async (
			values: SeniorityFormsValues,
			{ setSubmitting }: FormikHelpers<SeniorityFormsValues>
		) => {
			try {
				await handleSubmit(values);
			} finally {
				setSubmitting(false);
			}
		},
		[handleSubmit]
	);

	const minSnapshotDate = useMemo(() => {
		if (snapshotDate) {
			return dayjs(snapshotDate).clone().add({ day: 1 });
		}

		return currentDate;
	}, [snapshotDate]);

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialValues}
			validationSchema={skillsValidationSchema}
			onSubmit={handleFormSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={FORM_NAME}
			id={userId}
		>
			{({
				setFieldValue,
				setFieldTouched,
				submitForm,
				validateField,
				isSubmitting,
				values,
				errors,
				touched,
				handleOutsideClose,
			}) => {
				const { skills, date } = values;

				const setField = (field: string, value: unknown) => {
					setFieldValue(field, value);
					setFieldTouched(field, true);
					validateField(field);
				};

				const handleDateChange = (value: Dayjs | null) => {
					setField('date', value);
				};

				const skillsTouched =
					typeof touched.skills !== 'string'
						? (touched as SkillsTouched)?.skills || []
						: [];

				const softSkillsErrors = extractSkillsErrors(errors);

				const handleSeniorityChange =
					(index: number) => (newSeniority: SeniorityLevelType | null) => {
						const info = skills[index];

						setField(`skills.${index}`, {
							...info,
							seniority: newSeniority,
						});
					};

				return (
					<PopUp
						title={
							editSnapshotId
								? getEditingPopUpTitle(seniorityEntityName)
								: getAddingPopUpTitle(seniorityEntityName)
						}
						onClose={handleClose}
						onShadowClick={handleOutsideClose}
						controls={{
							positiveControl: { onClick: submitForm },
							negativeControl: { onClick: handleClose },
						}}
						type="form"
						showLoader={isSubmitting || isLoading || isFetching}
						stylesConfig={popUpStylesConfig}
					>
						<FormWrapper>
							<DatePicker
								innerLabel="Date"
								isRequired
								onChange={handleDateChange}
								errorText={
									touched.date && errors.date ? String(errors.date) : ''
								}
								value={dayjs(date)}
								disabled={!!editSnapshotId || !snapshotDate}
								maxDate={currentDate}
								minDate={minSnapshotDate}
							/>
							{skills && skills.length > 0 && (
								<SkillsList
									onSeniorityChange={handleSeniorityChange}
									skills={skills}
									errors={softSkillsErrors}
									touched={skillsTouched}
								/>
							)}
						</FormWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
