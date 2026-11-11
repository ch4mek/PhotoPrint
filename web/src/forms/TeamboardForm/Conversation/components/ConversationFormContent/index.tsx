import { useTheme } from '@emotion/react';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { Note } from '@mms/mms-ui-library/components/Note';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { ChangeEvent, useCallback, useMemo } from 'react';

import {
	CONVERSATION_FORM_FIELDS,
	PASSED_STATUS_OPTIONS,
} from '../../constants';
import { ConversationFormWrapper, DatePickersWrapper } from '../../styles';
import { ConversationFormField, ConversationType } from '../../types';
import { getPopUpStylesConfig } from '../../utils';
import { getConversationTypeOptions } from '../../utils/getConversationTypeOptions';

import { AttachFilesGroup } from '@/components/AttachFilesGroup';
import { MAX_NUMBER_OF_FILES } from '@/components/AttachFilesGroup/constants';
import { DynamicFieldsGroup } from '@/components/DynamicFieldsGroup';
import { DEFAULT_MAXIMUM_ALLOWED_DOCUMENT_SIZE_KB } from '@/constants/allowedFileSizes';
import { feedbackAutoselectOptions } from '@/constants/feedback';
import { attachedAllFileExtension } from '@/constants/fileExtensions';
import { useAuth } from '@/context/AuthContext';
import { TextArea } from '@/fields/TextArea';
import { formatConversationFileType } from '@/forms/TeamboardForm/Conversation/utils/formatConversationFileType';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { getFileNameWithExtension } from '@/helpers/getFileNameWithExtension';
import { useAttachDocument } from '@/hooks/useAttachDocument';
import { useResize } from '@/hooks/useResize';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { DevelopmentPlanSection } from '@/pages/TeamBoardUser/components/TeamBoardUserSummaryTab/DevelopmentPlanSection';
import { SalarySection } from '@/pages/TeamBoardUser/components/TeamBoardUserSummaryTab/SalarySection';
import { UserClaims } from '@/types/claims';
import { FormAutoselectOption } from '@/types/forms';
import { TeamboardTableMode } from '@/types/teamboard';

import { ConversationFormContentProps } from './types';
import { getNoteMessage } from './utils';

export const ConversationFormContent = ({
	formikProps,
	conversationPopUpTitle,
	isFetching,
	isLoading,
	isEditMode,
	initialValues,
	initialConversationValues,
	userSalaries,
	developmentPlans,
	isHRAccess,
	isGroupManagerRole,
	id,
	onCloseConversationPopUp,
	onFileGroups,
	onUserSalaries,
	onDevelopmentPlans,
	isIntern,
}: ConversationFormContentProps) => {
	const {
		setFieldValue,
		setFieldTouched,
		validateForm,
		validateField,
		submitForm,
		isSubmitting,
		setTouched,
		errors,
		touched,
		values,
		handleOutsideClose,
	} = formikProps;

	const {
		date,
		files,
		conversationType,
		conversationResult,
		performanceReviewDate,
		passed,
	} = values;

	const attachedFilesForDisplay = useMemo(
		() =>
			files.map(({ fileName, fileType, ...file }) => ({
				...file,
				fileName: fileType
					? getFileNameWithExtension(fileName, fileType)
					: fileName,
			})),
		[files]
	);

	const theme = useTheme();
	const { claims } = useAuth();
	const [, windowHeight] = useResize();

	const { handleFileChange } = useAttachDocument(
		attachedAllFileExtension,
		DEFAULT_MAXIMUM_ALLOWED_DOCUMENT_SIZE_KB
	);

	const handleDateChange = useCallback(
		(fieldName: string) => async (value: Dayjs | null) => {
			await setFieldValue(fieldName, value, true);
			setFieldTouched(fieldName, true);
		},
		[]
	);

	const handleAutoselectChange =
		(field: string) =>
		(value: FormAutoselectOption<string | number> | null) => {
			setFieldValue(field, value);
			setFieldTouched(field, true);
			validateField(field);

			if (field === ConversationFormField.TYPE && value) {
				if (
					value.id === ConversationType.Conversation ||
					value.id === ConversationType.HRConversation
				) {
					setFieldValue(
						ConversationFormField.PERFORMANCE_DATE,
						initialValues.performanceReviewDate
					);
					setFieldValue(ConversationFormField.PASSED, initialValues.passed);
				} else {
					setFieldValue(ConversationFormField.PASSED, PASSED_STATUS_OPTIONS[1]);
					setFieldValue(
						ConversationFormField.RESULT,
						initialValues.conversationResult
					);
				}
				setTouched({}, false);
				validateForm();
			}
		};

	const handleAttachedFileChange = () => (e: ChangeEvent<HTMLInputElement>) => {
		const selectedFiles = handleFileChange(
			e,
			files.length,
			MAX_NUMBER_OF_FILES
		);

		if (selectedFiles) {
			const newFiles = selectedFiles.map((selectedFile) => ({
				fileId: `${files.length}-${selectedFile?.name}`,
				fileName: selectedFile?.name,
				fileSize: selectedFile.size,
				fileType: formatConversationFileType(selectedFile.name),
				file: selectedFile,
			}));

			setFieldValue(CONVERSATION_FORM_FIELDS.files.name, [
				...files,
				...newFiles,
			]);

			onFileGroups((prevState) => [...prevState, ...newFiles]);
		}

		return selectedFiles;
	};

	const handleAttachedFileRemove = (removedFileId: string | number) => () => {
		const updatedFiles = files.filter(({ fileId }) => fileId !== removedFileId);

		setFieldValue(CONVERSATION_FORM_FIELDS.files.name, updatedFiles);
		onFileGroups((prevState) =>
			prevState.filter(({ fileId }) => fileId !== removedFileId)
		);
	};

	const handleTextFieldChange = (fieldName: string, value: string) => {
		setFieldValue(fieldName, value, true);
		setFieldTouched(fieldName, true);
	};

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	const hasHRModuleClaim = checkIsHasClaims(
		[UserClaims.ManageHRModule],
		claims
	);

	const conversationTypeOptions = useMemo(
		() => getConversationTypeOptions(hasHRModuleClaim, isIntern),
		[hasHRModuleClaim, isIntern]
	);

	const isPerformanceType =
		conversationType?.id === ConversationType.PerformanceReview;

	const isConversationType =
		conversationType?.id === ConversationType.Conversation;

	const shouldDisableConversationType =
		isHRAccess ||
		isEditMode ||
		Boolean(initialConversationValues?.conversationType) ||
		conversationTypeOptions.length === 1;

	const isHRConversationType =
		conversationType?.id === ConversationType.HRConversation;

	const isHROrConversationType = isHRConversationType || isConversationType;

	return (
		<PopUp
			type="form"
			title={conversationPopUpTitle}
			onClose={onCloseConversationPopUp}
			onShadowClick={handleOutsideClose}
			controls={{
				positiveControl: { onClick: submitForm },
				negativeControl: { onClick: onCloseConversationPopUp },
			}}
			showLoader={isFetching || isLoading || isSubmitting}
			stylesConfig={popUpStylesConfig}
		>
			<ConversationFormWrapper>
				<DatePickersWrapper>
					<DatePicker
						{...CONVERSATION_FORM_FIELDS.date}
						size="fullWidth"
						errorText={
							(touched.date || date) && errors.date ? String(errors.date) : ''
						}
						disabled={isEditMode}
						value={date ? dayjs(date) : null}
						onChange={handleDateChange(ConversationFormField.DATE)}
						isRequired
					/>
					<Autoselect
						{...CONVERSATION_FORM_FIELDS.conversationType}
						value={conversationType}
						options={conversationTypeOptions}
						onChange={handleAutoselectChange(ConversationFormField.TYPE)}
						error={
							Boolean(errors.conversationType) &&
							Boolean(touched.conversationType)
						}
						errorText={String(errors.conversationType)}
						disableClearable
						disabled={shouldDisableConversationType}
						isRequired
					/>
					{isHROrConversationType && (
						<Autoselect
							{...CONVERSATION_FORM_FIELDS.conversationResult}
							value={conversationResult}
							options={feedbackAutoselectOptions}
							onChange={handleAutoselectChange(ConversationFormField.RESULT)}
							error={
								Boolean(errors.conversationResult) && touched.conversationResult
							}
							errorText={errors.conversationResult}
							isRequired
						/>
					)}
					{isPerformanceType && (
						<>
							<DatePicker
								{...CONVERSATION_FORM_FIELDS.performanceReviewDate}
								size="fullWidth"
								errorText={
									touched.performanceReviewDate && errors.performanceReviewDate
										? String(errors.performanceReviewDate)
										: ''
								}
								value={
									performanceReviewDate ? dayjs(performanceReviewDate) : null
								}
								onChange={handleDateChange(
									ConversationFormField.PERFORMANCE_DATE
								)}
								isRequired
							/>

							<Autoselect
								{...CONVERSATION_FORM_FIELDS.passed}
								value={passed}
								options={PASSED_STATUS_OPTIONS}
								onChange={handleAutoselectChange(ConversationFormField.PASSED)}
								error={Boolean(errors.passed) && touched.passed}
								errorText={errors.passed}
								disableClearable
								isRequired
							/>
						</>
					)}
				</DatePickersWrapper>
				<TextArea
					{...CONVERSATION_FORM_FIELDS.theses}
					size="fullWidth"
					error={Boolean(errors.theses) && touched.theses}
					errorText={String(errors.theses)}
					isRequired={isConversationType}
					onChange={handleTextFieldChange}
				/>
				<TextArea
					{...CONVERSATION_FORM_FIELDS.employeeTheses}
					size="fullWidth"
					isRequired={isConversationType}
					onChange={handleTextFieldChange}
				/>
				<TextArea
					{...CONVERSATION_FORM_FIELDS.agreements}
					size="fullWidth"
					isRequired={isConversationType}
					onChange={handleTextFieldChange}
				/>
				{!isGroupManagerRole && !isHRConversationType && (
					<SalarySection
						userSalaries={userSalaries}
						setUserSalaries={onUserSalaries}
						mode={TeamboardTableMode.PopUp}
						conversationId={id}
					/>
				)}
				{!isIntern && !isHRConversationType && (
					<DevelopmentPlanSection
						mode={TeamboardTableMode.PopUp}
						developmentPlans={developmentPlans}
						setDevelopmentPlans={onDevelopmentPlans}
						conversationId={id}
					/>
				)}
				<DynamicFieldsGroup {...CONVERSATION_FORM_FIELDS.documentLinks} />
				<AttachFilesGroup
					files={attachedFilesForDisplay}
					onAttachedFileRemove={handleAttachedFileRemove}
					onAttachedFileChange={handleAttachedFileChange}
					isAttachMultiple
					shouldShowFileType={false}
				/>
				{!isHRConversationType && (
					<Note>{getNoteMessage(isGroupManagerRole, isIntern)}</Note>
				)}
			</ConversationFormWrapper>
		</PopUp>
	);
};
