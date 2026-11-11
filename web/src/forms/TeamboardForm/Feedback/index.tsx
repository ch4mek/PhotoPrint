import { useTheme } from '@emotion/react';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useMemo } from 'react';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import { feedbackAutoselectOptions } from '@/constants/feedback';
import { TextField } from '@/fields/Text';
import { TextArea } from '@/fields/TextArea';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormWrapper } from '@/forms/styles';
import { useResize } from '@/hooks/useResize';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { useUserConverstaions } from '@/queries/teamboard/useUserConverstaions';
import { useUserFeedbackById } from '@/queries/teamboard/useUserFeedbackById';
import { useUserProjects } from '@/queries/useUserProjects';

import {
	conversationQueryParams,
	entityName,
	FEEDBACK_FORM_INITIAL_VALUES,
	feedbackFormFields,
	formName,
} from './constants';
import { feedbackValidationSchema } from './schema';
import {
	DateAndTypeInputsWrapper,
	FirstInputsWrapper,
	LastInputsWrapper,
} from './styles';
import { FeedbackFormProps, FeedbackFormValues } from './types';
import {
	convertConversationResponseToOptions,
	convertFeedbackToFormInitialState,
	getPopUpStylesConfig,
} from './utils';

export function FeedbackForm({
	handleClose,
	onSubmit,
	id,
	userId,
}: FeedbackFormProps) {
	const { data, isLoading } = useUserFeedbackById(id);
	const [, windowHeight] = useResize();
	const isEditMode = Boolean(id);

	const { data: privateUserProjects, isLoading: areProjectsLoading } =
		useUserProjects(
			userId,
			{
				isActive: true,
			},
			{
				enabled: !isEditMode,
			}
		);

	const { data: conversationOptionsData, isLoading: areConversationsLoading } =
		useUserConverstaions({
			userId,
			...conversationQueryParams,
		});

	const conversationOptions = useMemo(() => {
		if (conversationOptionsData?.items) {
			return convertConversationResponseToOptions(
				conversationOptionsData.items
			);
		}

		return [];
	}, [conversationOptionsData]);

	const initialValues: FeedbackFormValues = useMemo(() => {
		if (id && data) {
			return convertFeedbackToFormInitialState(data, conversationOptions);
		}

		return FEEDBACK_FORM_INITIAL_VALUES;
	}, [data, id, conversationOptions]);

	const isFormLoading =
		isLoading || areConversationsLoading || areProjectsLoading;

	const theme = useTheme();

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialValues}
			validationSchema={feedbackValidationSchema}
			onSubmit={onSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={formName}
			id={`${userId}_${id}`}
		>
			{({
				setFieldValue,
				setFieldTouched,
				validateField,
				submitForm,
				isSubmitting,
				errors,
				touched,
				values,
				handleOutsideClose,
			}) => {
				const handleDateChange =
					(fieldName: string) => (value: Dayjs | null) => {
						if (value === null) {
							setFieldValue(fieldName, null, true);

							return;
						}

						setFieldValue(fieldName, value, true);
						setFieldTouched(fieldName, true);
					};

				const handleAutoselectChange = (field: string) => (value: unknown) => {
					setFieldValue(field, value);
					setFieldTouched(field, true);
					validateField(field);
				};

				const handleInputChange = (fieldName: string, fieldValue: string) => {
					setFieldValue(fieldName, fieldValue, true);
					setFieldTouched(fieldName, true);
				};

				const { date } = values;

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
						showLoader={isFormLoading || isSubmitting}
						stylesConfig={popUpStylesConfig}
					>
						<FormWrapper>
							<FirstInputsWrapper>
								<DateAndTypeInputsWrapper>
									<DatePicker
										{...feedbackFormFields.date}
										disabled={isEditMode}
										errorText={
											touched.date && errors.date ? String(errors.date) : ''
										}
										value={dayjs(date)}
										onChange={handleDateChange('date')}
										isRequired
										size="fullWidth"
									/>
									<Autoselect
										{...feedbackFormFields.type}
										value={values.type}
										options={feedbackAutoselectOptions}
										onChange={handleAutoselectChange('type')}
										error={Boolean(errors.type) && touched.type}
										errorText={errors.type}
										isRequired
									/>
								</DateAndTypeInputsWrapper>
								<Autoselect
									{...feedbackFormFields.project}
									value={values.project}
									options={privateUserProjects || []}
									disabled={isEditMode}
									onChange={handleAutoselectChange('project')}
									error={Boolean(errors.project) && touched.project}
									errorText={errors.project}
									isRequired
								/>
							</FirstInputsWrapper>
							<TextArea
								{...feedbackFormFields.feedback}
								disabled={isEditMode}
								onChange={handleInputChange}
								isRequired
							/>
							<TextArea {...feedbackFormFields.actions} />
							<LastInputsWrapper>
								<Autoselect
									{...feedbackFormFields.conversation}
									value={values.conversation}
									options={conversationOptions}
									onChange={handleAutoselectChange('conversation')}
									error={Boolean(errors.conversation) && touched.conversation}
									errorText={errors.conversation}
								/>
								<TextField
									{...feedbackFormFields.reviewer}
									onChange={handleInputChange}
								/>
							</LastInputsWrapper>
						</FormWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
