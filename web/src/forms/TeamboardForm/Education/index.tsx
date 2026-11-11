import { useTheme } from '@emotion/react';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useQueryClient } from '@tanstack/react-query';
import { FormikHelpers } from 'formik';
import { useCallback, useContext, useMemo } from 'react';

import { FormWrapper } from '../../styles';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import {
	getSuccessfullyAddedMessage,
	getSuccessfullyUpdatedMessage,
	NotificationType,
} from '@/constants/notifications';
import { EducationsQueriesKeys } from '@/constants/queriesKeys';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { TextField } from '@/fields/Text';
import { TextArea } from '@/fields/TextArea';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { setNumbersType1Value } from '@/helpers/setNumbersType1Value';
import { useResize } from '@/hooks/useResize';
import { PersonalDataContext } from '@/pages/TeamBoardUser/context/personalDataContext';
import { useAddEducation } from '@/queries/educations/useAddEducation';
import { useUpdateEducation } from '@/queries/educations/useUpdateEducation';

import {
	EDUCATION_FORM_FIELDS,
	EDUCATION_TITLE,
	EMPTY_FORM_VALUES,
} from './constants';
import { validationSchema } from './schema';
import { EducationFormProps, EducationFormValues } from './types';
import { getEducationFormValuesFromResponse } from './utils/getEducationFormValuesFromResponse';
import { getPopUpStylesConfig } from './utils/getPopUpStylesConfig';

export function EducationForm({
	education = null,
	closePopup,
}: EducationFormProps) {
	const theme = useTheme();
	const toast = useToast();
	const queryClient = useQueryClient();
	const [, windowHeight] = useResize();

	const {
		persistentValues: { studyFormTypes, educationLevelTypes },
	} = useConfiguration();
	const { userId } = useContext(PersonalDataContext);

	const initialFormValues: EducationFormValues = useMemo(
		() =>
			education
				? getEducationFormValuesFromResponse(education, {
						educationLevelTypes,
						studyFormTypes,
				  })
				: EMPTY_FORM_VALUES,
		[education]
	);

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	const { mutateAsync: addEducation } = useAddEducation({
		userId: Number(userId),
		options: {
			onError: createOnErrorHandler(toast),
			onSuccess: () => {
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						getSuccessfullyAddedMessage(EDUCATION_TITLE)
					)
				);
				queryClient.invalidateQueries({
					queryKey: [EducationsQueriesKeys.userEducations],
				});
				closePopup();
			},
		},
	});

	const { mutateAsync: updateEducation } = useUpdateEducation({
		userId: Number(userId),
		educationId: Number(education?.id),
		options: {
			onError: createOnErrorHandler(toast),
			onSuccess: () => {
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						getSuccessfullyUpdatedMessage(EDUCATION_TITLE)
					)
				);
				queryClient.invalidateQueries({
					queryKey: [EducationsQueriesKeys.userEducations],
				});
				closePopup();
			},
		},
	});

	const mutateEducation = useCallback(
		education ? updateEducation : addEducation,
		[education, updateEducation, addEducation]
	);

	const handleSubmit = useCallback(
		async (
			values: EducationFormValues,
			{ setSubmitting }: FormikHelpers<EducationFormValues>
		) => {
			await mutateEducation(values);
			setSubmitting(false);
		},
		[mutateEducation]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialFormValues}
			validationSchema={validationSchema}
			validateOnBlur={false}
			onSubmit={handleSubmit}
			onClose={closePopup}
			name={EDUCATION_TITLE}
			id={education?.id}
		>
			{({
				values,
				setFieldValue,
				setFieldTouched,
				submitForm,
				errors,
				touched,
				isSubmitting,
				handleOutsideClose,
			}) => {
				const {
					educationLevel,
					educationalInstitution,
					faculty,
					specialization,
					studyForm,
					graduationYear,
				} = values;

				const handleOnChange =
					(fieldName: keyof EducationFormValues) => (value: unknown) => {
						setFieldValue(fieldName, value, true);
						setFieldTouched(fieldName, true);
					};

				return (
					<PopUp
						title={
							education
								? getEditingPopUpTitle(EDUCATION_TITLE)
								: getAddingPopUpTitle(EDUCATION_TITLE)
						}
						onClose={closePopup}
						onShadowClick={handleOutsideClose}
						controls={{
							positiveControl: { onClick: submitForm },
							negativeControl: { onClick: closePopup },
						}}
						type="form"
						showLoader={isSubmitting}
						stylesConfig={popUpStylesConfig}
					>
						<FormWrapper>
							<Autoselect
								label="Education level"
								value={educationLevel}
								valueField="description"
								options={educationLevelTypes}
								onChange={handleOnChange('educationLevel')}
								error={Boolean(errors.educationLevel && touched.educationLevel)}
								errorText={errors.educationLevel}
								isRequired
							/>
							<TextField
								{...EDUCATION_FORM_FIELDS.educationalInstitution}
								onChange={setFieldValue}
								value={educationalInstitution}
								isRequired
							/>
							<TextField
								{...EDUCATION_FORM_FIELDS.faculty}
								onChange={setFieldValue}
								value={faculty}
								isRequired
							/>
							<TextField
								{...EDUCATION_FORM_FIELDS.specialization}
								onChange={setFieldValue}
								value={specialization}
								isRequired
							/>
							<Autoselect
								label="Study form"
								value={studyForm}
								valueField="description"
								options={studyFormTypes}
								onChange={handleOnChange('studyForm')}
								error={Boolean(errors.studyForm && touched.studyForm)}
								errorText={errors.studyForm}
								isRequired
							/>
							<TextField
								{...EDUCATION_FORM_FIELDS.graduationYear}
								onChange={setNumbersType1Value(setFieldValue)}
								value={graduationYear}
								isRequired
							/>
							<TextArea
								{...EDUCATION_FORM_FIELDS.comment}
								onChange={setFieldValue}
								size="fullWidth"
							/>
						</FormWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
