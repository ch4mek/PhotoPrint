import { useTheme } from '@emotion/react';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { FormikHelpers } from 'formik';
import { useCallback, useMemo } from 'react';

import { getSpecializationPopupStyles } from '../constants';

import { UserHardSkillList } from '@/components/UserHardSkillList';
import { getAddingPopUpTitle } from '@/constants/activityStatus';
import { specializationEntityName } from '@/constants/formEntities';
import { useConfiguration } from '@/context/Configuration';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormWrapper } from '@/forms/styles';
import { useResize } from '@/hooks/useResize';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { useGetUserSpecializationById } from '@/queries/teamboard/useGetUserSpecializationById';
import { useUserSpecializations } from '@/queries/teamboard/useUserNotAssignedSpecializations';
import { SeniorityLevelType } from '@/types/configuration';
import {
	FormSpecialization,
	UserSpecializationFormValues,
} from '@/types/teamboard';

import { DEFAULT_INITIAL_VALUES, FORM_NAME } from './constants';
import { useSpecializationFormSubmit } from './hooks/useSpecializationFormSubmit';
import { validationSchema } from './schema';
import { SpecializationTouched, UserSpecializationFormProps } from './types';
import {
	convertNotAssignedSpecializationsToOptions,
	extractSpecializationErrors,
} from './utils';

export function UserSpecializationForm({
	handleClose,
	userId,
	primarySpecializationId,
}: UserSpecializationFormProps) {
	const { data: notAssignedSpecializations } = useUserSpecializations(
		userId,
		false,
		{
			enabled: !!userId,
		}
	);
	const { data: primarySpecialization } = useGetUserSpecializationById(
		primarySpecializationId,
		{ enabled: primarySpecializationId !== undefined }
	);

	const {
		configurableValues: { seniorityLevels },
	} = useConfiguration();

	const [, windowHeight] = useResize();

	const handleSubmitForm = useSpecializationFormSubmit();

	const specializationOptions = useMemo(
		() =>
			notAssignedSpecializations
				? convertNotAssignedSpecializationsToOptions(notAssignedSpecializations)
				: [],
		[notAssignedSpecializations]
	);

	const seniorityOptions = [...seniorityLevels].reverse();

	const initialValues: UserSpecializationFormValues = useMemo(
		() => DEFAULT_INITIAL_VALUES,
		[primarySpecialization]
	);

	const handleSubmit = useCallback(
		async (
			values: UserSpecializationFormValues,
			{ setSubmitting }: FormikHelpers<UserSpecializationFormValues>
		) => {
			if (userId) {
				const isSubmitted = await handleSubmitForm(values, Number(userId));

				setSubmitting(false);

				if (isSubmitted) {
					handleClose();
				}
			}
		},
		[initialValues]
	);

	const theme = useTheme();

	const popUpStylesConfig = useMemo(
		() => getSpecializationPopupStyles(theme, windowHeight),
		[theme, windowHeight]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialValues}
			validationSchema={validationSchema}
			onSubmit={handleSubmit}
			onClose={handleClose}
			validateOnChange={false}
			validateOnBlur={false}
			validateOnMount={false}
			name={FORM_NAME}
			id={userId}
		>
			{({
				setFieldValue,
				setFieldError,
				setFieldTouched,
				submitForm,
				isSubmitting,
				values,
				errors,
				touched,
				handleOutsideClose,
			}) => {
				const { date, specialization } = values;

				const handleDateChange = (value: Dayjs | null) => {
					setFieldValue('date', value);
				};

				const specializationError =
					typeof errors?.specialization === 'string'
						? errors.specialization
						: undefined;

				const seniorityError =
					errors?.specialization &&
					typeof errors.specialization === 'object' &&
					'seniority' in errors.specialization &&
					typeof (errors.specialization as any).seniority === 'string'
						? (errors.specialization as any).seniority
						: undefined;

				const seniorityTouched =
					typeof touched.specialization !== 'string'
						? (touched as SpecializationTouched)?.specialization?.seniority
						: undefined;

				const hardSkillsTouched =
					typeof touched.specialization !== 'string'
						? (touched as SpecializationTouched)?.specialization?.hardSkills ||
						  []
						: [];

				const hardSkillsErrors = extractSpecializationErrors(errors);

				const handleSpecializationChange = (
					value: FormSpecialization | null
				) => {
					setFieldValue('specialization', value, true);
					setFieldTouched('specialization', true);

					if (errors.specialization) {
						setFieldError('specialization', undefined);
					}
				};

				const handleSeniorityChange = (
					selectedOption: SeniorityLevelType | null
				) => {
					setFieldValue('specialization.seniority', selectedOption, true);
					setFieldTouched('specialization.seniority', true);

					if (seniorityError) {
						setFieldError('specialization.seniority', undefined);
					}
				};

				const handleSpecializationSeniorityChange =
					(index: number) => (newSeniority: SeniorityLevelType | null) => {
						const fieldName = `specialization.hardSkills.${index}`;
						const errorFieldName = `specialization.hardSkills.${index}.seniority`;

						if (!specialization?.hardSkills[index]) {
							return;
						}

						const { info } = specialization.hardSkills[index];

						if (!newSeniority) {
							setFieldValue(fieldName, {
								info,
							});

							return;
						}

						setFieldValue(fieldName, {
							info,
							seniority: newSeniority,
						});

						if (hardSkillsErrors[index]?.seniority) {
							setFieldError(errorFieldName, undefined);
						}
					};

				return (
					<PopUp
						title={getAddingPopUpTitle(specializationEntityName)}
						onClose={handleClose}
						onShadowClick={handleOutsideClose}
						controls={{
							positiveControl: { onClick: submitForm },
							negativeControl: { onClick: handleClose },
						}}
						type="form"
						showLoader={isSubmitting}
						stylesConfig={popUpStylesConfig}
					>
						<FormWrapper>
							<DatePicker
								innerLabel="Date"
								isRequired
								onChange={handleDateChange}
								value={dayjs(date)}
								disabled
							/>
							<Autoselect
								value={specialization}
								options={specializationOptions || []}
								label="Specialization"
								valueField="name"
								onChange={handleSpecializationChange}
								error={Boolean(specializationError) && touched.specialization}
								errorText={specializationError}
								isRequired
							/>
							{specialization && (
								<Autoselect
									label="Seniority"
									value={specialization.seniority ?? null}
									valueField="name"
									onChange={handleSeniorityChange}
									options={seniorityOptions}
									error={Boolean(seniorityError) && seniorityTouched}
									errorText={seniorityError}
									isRequired
								/>
							)}
							{specialization && specialization.hardSkills.length > 0 && (
								<UserHardSkillList
									hardSkills={specialization.hardSkills}
									onSeniorityChange={handleSpecializationSeniorityChange}
									errors={hardSkillsErrors}
									touched={hardSkillsTouched}
									senioritiesDisabled={
										specialization?.id === initialValues.specialization?.id
									}
								/>
							)}
						</FormWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
