import { useTheme } from '@emotion/react';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { FormikHelpers } from 'formik';
import { useCallback, useMemo } from 'react';

import { getSpecializationPopupStyles } from '../constants';

import { CollapsibleHardSkillList } from '@/components/CollapsibleHardSkillList';
import {
	getEditingPopUpTitle,
	getAddingPopUpTitle,
} from '@/constants/activityStatus';
import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { useConfiguration } from '@/context/Configuration';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormWrapper } from '@/forms/styles';
import { useResize } from '@/hooks/useResize';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { useGetUserSpecializationSnapshotById } from '@/queries/teamboard/useGetUserSpecializationSnapshotById';
import { useUserSpecializations } from '@/queries/teamboard/useUserNotAssignedSpecializations';
import { SeniorityLevelType } from '@/types/configuration';

import { entityName, formName } from './constants';
import { validationSchema } from './schema';
import {
	UserSpecializationsFormValues,
	UserSpecializationsFormProps,
} from './types';
import {
	extractErrors,
	extractSeniorityError,
	extractSpecializationsErrors,
	extractSpecializationsTouched,
} from './utils';

export function UserSpecializationsForm({
	isEditMode,
	handleClose,
	userId,
	snapshotId,
	onSubmit,
	minDate,
}: UserSpecializationsFormProps) {
	const {
		data: specializationLookup,
		isLoading: isSpecializationLookupLoading,
		isFetching: isSpecializationLookupFetching,
	} = useUserSpecializations(userId, true, {
		enabled: Boolean(userId) && !isEditMode,
		queryKey: [TeamboardQueriesKeys.userSpecializationsLookupForm, userId],
	});
	const {
		data: snapshotData,
		isLoading: isSnapshotDataLoading,
		isFetching: isSnapshotDataFetching,
	} = useGetUserSpecializationSnapshotById(snapshotId);
	const [, windowHeight] = useResize();

	const {
		configurableValues: { seniorityLevels },
	} = useConfiguration();

	const initialValues: UserSpecializationsFormValues = useMemo(() => {
		if (snapshotData) {
			if (isEditMode) {
				const { snapshotDate, userSpecializations } = snapshotData;

				return {
					snapshotDate: dayjs(snapshotDate).startOf('day'),
					userSpecializations: userSpecializations.map(
						({
							specializationTemplateId,
							specializationTemplateName,
							hardSkills,
							seniorityLevelId: senioritySpecializationLevelId,
							seniorityLevelName: senioritySpecializationLevelName,
						}) => ({
							id: specializationTemplateId,
							name: specializationTemplateName,
							hardSkills: hardSkills.map(
								({ id, name, seniorityLevelId, seniorityLevelName }) => ({
									seniority:
										seniorityLevelId && seniorityLevelName
											? { id: seniorityLevelId, name: seniorityLevelName }
											: null,
									info: { id, name },
								})
							),
							seniority: senioritySpecializationLevelId
								? {
										id: senioritySpecializationLevelId,
										name: senioritySpecializationLevelName,
								  }
								: null,
						})
					),
				};
			}

			if (specializationLookup) {
				return {
					snapshotDate: dayjs().startOf('day'),
					userSpecializations: specializationLookup.map(
						({
							specializationTemplateId,
							specializationTemplateName,
							hardSkills,
						}) => {
							const currentSpecPrevValues =
								snapshotData.userSpecializations.find(
									(prevSnapshotSpec) =>
										prevSnapshotSpec.specializationTemplateId ===
										specializationTemplateId
								);

							return {
								id: specializationTemplateId,
								name: specializationTemplateName,
								hardSkills: hardSkills.map(({ id, name }) => {
									const currentHardSkillsPrevValues =
										currentSpecPrevValues?.hardSkills.find(
											(prevSnapshotHardSkill) => prevSnapshotHardSkill.id === id
										);
									const info = { id, name };

									if (currentHardSkillsPrevValues) {
										const { seniorityLevelId, seniorityLevelName } =
											currentHardSkillsPrevValues;

										return {
											seniority: {
												id: seniorityLevelId,
												name: seniorityLevelName,
											},
											info,
										};
									}

									return {
										seniority: null,
										info,
									};
								}),
								seniority: currentSpecPrevValues?.seniorityLevelId
									? {
											id: currentSpecPrevValues?.seniorityLevelId,
											name: currentSpecPrevValues.seniorityLevelName,
									  }
									: null,
							};
						}
					),
				};
			}
		}

		return {
			snapshotDate: dayjs().startOf('day'),
			userSpecializations: [],
		};
	}, [snapshotData, specializationLookup, isEditMode]);

	const handleSubmit = useCallback(
		async (
			values: UserSpecializationsFormValues,
			{ setSubmitting }: FormikHelpers<UserSpecializationsFormValues>
		) => {
			await onSubmit(values, snapshotData ? snapshotData.id : null);
			setSubmitting(false);
			handleClose();
		},
		[snapshotData]
	);

	const theme = useTheme();

	const popUpStylesConfig = useMemo(
		() => getSpecializationPopupStyles(theme, windowHeight),
		[theme, windowHeight]
	);

	const isDataLoading =
		isSpecializationLookupFetching ||
		isSpecializationLookupLoading ||
		isSnapshotDataFetching ||
		isSnapshotDataLoading;

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialValues}
			validationSchema={validationSchema}
			onSubmit={handleSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={formName}
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
				const handleOnChange = (field: string) => (value: unknown) => {
					setFieldValue(field, value);
					setFieldTouched(field, true);
					validateField(field);
				};

				const handleOnDateChange = (field: string) => (value: Dayjs | null) => {
					setFieldValue(field, value);
					setFieldTouched(field, true);
					validateField(field);
				};

				const { snapshotDate, userSpecializations } = values;

				const handleSeniorityChange =
					(specializationIndex: number) => (newValue: string | null) => {
						const selected = seniorityLevels.find((s) => s.name === newValue);

						const fieldName = `userSpecializations.${specializationIndex}.seniority`;

						setFieldValue(fieldName, selected || null);
					};

				const handleSpecializationSeniorityChange =
					(specializationIndex: number) =>
					(hardSkillIndex: number) =>
					(newSeniority: SeniorityLevelType | null) => {
						const fieldName = `userSpecializations.${specializationIndex}.hardSkills.${hardSkillIndex}`;

						if (
							!userSpecializations[specializationIndex]?.hardSkills[
								hardSkillIndex
							]
						) {
							return;
						}

						const { info } =
							userSpecializations[specializationIndex].hardSkills[
								hardSkillIndex
							];

						if (!newSeniority) {
							handleOnChange(fieldName)({
								info,
							});
						}

						handleOnChange(fieldName)({
							info,
							seniority: newSeniority,
						});
					};

				const extractedErrors = extractErrors(errors);

				return (
					<PopUp
						title={
							isEditMode
								? getEditingPopUpTitle(entityName)
								: getAddingPopUpTitle(entityName)
						}
						onClose={handleClose}
						onShadowClick={handleOutsideClose}
						controls={{
							positiveControl: { onClick: submitForm },
							negativeControl: { onClick: handleClose },
						}}
						type="form"
						showLoader={isDataLoading || isSubmitting}
						stylesConfig={popUpStylesConfig}
					>
						<FormWrapper>
							<DatePicker
								innerLabel="Date"
								isRequired
								errorText={
									touched.snapshotDate ? String(errors.snapshotDate ?? '') : ''
								}
								onChange={handleOnDateChange('snapshotDate')}
								value={dayjs(snapshotDate)}
								disabled={isEditMode}
								maxDate={dayjs()}
								minDate={dayjs(minDate)}
							/>
							{userSpecializations.map(
								({ hardSkills, name, id, seniority }, index) => (
									<CollapsibleHardSkillList
										key={id}
										specialization={name}
										hardSkills={hardSkills}
										onSpecializationSeniorityChange={handleSpecializationSeniorityChange(
											index
										)}
										seniority={seniority}
										onSeniorityChange={handleSeniorityChange(index)}
										seniorityError={extractSeniorityError(
											extractedErrors,
											index
										)}
										errors={extractSpecializationsErrors(
											extractedErrors,
											index
										)}
										touched={extractSpecializationsTouched(touched, index)}
									/>
								)
							)}
						</FormWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
