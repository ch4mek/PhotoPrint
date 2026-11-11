import { FormikHelpers } from 'formik';
import { useCallback, useContext, useMemo, useState } from 'react';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import { isGroupManager } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { ConversationWarning } from '@/forms/TeamboardForm/Conversation/components/ConversationWarning';
import { convertIdsToConversationFiles } from '@/forms/TeamboardForm/Conversation/utils/convertIdsToConversationFiles';
import { getFilesToUpload } from '@/forms/TeamboardForm/Conversation/utils/getFilesToUpload';
import { getPreviousConversationFiles } from '@/forms/TeamboardForm/Conversation/utils/getPreviousConversationFiles';
import { PersonalDataContext } from '@/pages/TeamBoardUser/context/personalDataContext';
import { TeamboardUserProfileKind } from '@/pages/TeamBoardUser/types';
import { isLimitedHRAccess } from '@/pages/TeamBoardUser/utils';
import { useUploadFiles } from '@/queries/files/useUploadFiles';
import { useUserConversations } from '@/queries/teamboard/useUserConversations';
import { ConversationFormDocument } from '@/types/document';

import { ConversationFormContent } from './components/ConversationFormContent';
import {
	CONVERSATION_FORM_INITIAL_VALUES,
	CONVERSATION_TYPE_OPTIONS,
	EntityName,
	SKIP,
	TOP,
	FORM_NAME,
} from './constants';
import { conversationValidationSchema } from './schema';
import {
	ConversationFormProps,
	ConversationFormValues,
	ConversationType,
	DevelopmentPlanPopUp,
	UserSalaryPopUp,
} from './types';
import { convertConversationToFormInitialState } from './utils';

export function ConversationForm({
	handleClose,
	onSubmit,
	id,
	initialValues: initialConversationValues,
}: ConversationFormProps) {
	const { role } = useAuth();
	const isGroupManagerRole = isGroupManager(role);
	const isEditMode = Boolean(id);
	const { userId, accessLevel, profileKind } = useContext(PersonalDataContext);

	const isIntern = profileKind === TeamboardUserProfileKind.Intern;

	const isHRAccess = isLimitedHRAccess(accessLevel);

	const [userSalaries, setUserSalaries] = useState<UserSalaryPopUp[]>([]);
	const [developmentPlans, setDevelopmentPlans] = useState<
		DevelopmentPlanPopUp[]
	>([]);

	const [fileGroups, setFileGroups] = useState<ConversationFormDocument[]>([]);

	const [isWarningDialogOpen, setIsWarningDialogOpen] = useState(false);
	const handleToggleDialog = useCallback(
		() => setIsWarningDialogOpen((prev) => !prev),
		[]
	);

	const handleCloseAllPopUps = useCallback(() => {
		handleToggleDialog();
		handleClose();
	}, [handleToggleDialog, handleClose]);

	const handleCloseConversationPopUp = useCallback(() => {
		if (userSalaries.length > 0 || developmentPlans.length > 0) {
			handleToggleDialog();
		} else {
			handleClose();
		}
	}, [handleToggleDialog, handleClose, userSalaries, developmentPlans]);

	const { data, isFetching, isLoading } = useUserConversations(
		{
			userId,
			skip: SKIP,
			top: TOP,
			filters: {
				id,
			},
		},
		{
			enabled: Boolean(id),
		}
	);

	const { mutateAsync: uploadFiles } = useUploadFiles();

	const initialValues: ConversationFormValues = useMemo(() => {
		if (isEditMode && data && data.items[0]) {
			return convertConversationToFormInitialState(data.items[0]);
		}

		if (isHRAccess) {
			return {
				...CONVERSATION_FORM_INITIAL_VALUES,
				conversationType:
					CONVERSATION_TYPE_OPTIONS.find(
						(option) => option.id === ConversationType.HRConversation
					) || CONVERSATION_FORM_INITIAL_VALUES.conversationType,
			};
		}

		return {
			...CONVERSATION_FORM_INITIAL_VALUES,
			...initialConversationValues,
		};
	}, [isEditMode, data, isHRAccess, initialConversationValues]);

	const handleSubmit = useCallback(
		async (
			{
				passed,
				conversationType,
				conversationResult,
				...values
			}: ConversationFormValues,
			{ setSubmitting }: FormikHelpers<ConversationFormValues>
		) => {
			try {
				setSubmitting(true);
				const submissionData = {
					...values,
					passed: passed === null ? passed : Boolean(passed.id),
					conversationType: conversationType?.id || null,
					conversationResult: conversationResult?.id || null,
					userSalaries,
					developmentPlans,
					userPerformanceReviewId:
						initialConversationValues?.performanceReviewId,
					documentLinks: values.documentLinks.filter((item) => item.trim()),
				};

				const isPassedUpdatedToTrue = !!(
					passed?.id && !initialValues.passed?.id
				);

				const fileToUpload = getFilesToUpload(fileGroups);

				if (fileToUpload?.length) {
					await uploadFiles(fileToUpload, {
						onSuccess: async (res, variables: any) => {
							if (!res) {
								return;
							}

							const uploadedFiles = convertIdsToConversationFiles(
								res || [],
								variables
							);
							const previousConversationFiles = getPreviousConversationFiles(
								values.files
							);

							await onSubmit(
								{
									...submissionData,
									files: [...previousConversationFiles, ...uploadedFiles],
								},
								isPassedUpdatedToTrue
							);
						},
					});

					return;
				}

				onSubmit(
					{
						...submissionData,
						files: getPreviousConversationFiles(values.files),
					},
					isPassedUpdatedToTrue
				);
			} finally {
				setSubmitting(false);
			}
		},
		[userSalaries, developmentPlans, initialValues, fileGroups]
	);

	const conversationPopUpTitle = useMemo(
		() =>
			isEditMode
				? getEditingPopUpTitle(EntityName)
				: getAddingPopUpTitle(EntityName),
		[isEditMode]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialValues}
			validationSchema={conversationValidationSchema}
			onSubmit={handleSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={FORM_NAME}
			id={`${userId}_${id}`}
		>
			{(formikProps) => (
				<>
					<ConversationFormContent
						{...{
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
							isIntern,
						}}
						onCloseConversationPopUp={handleCloseConversationPopUp}
						onDevelopmentPlans={setDevelopmentPlans}
						onFileGroups={setFileGroups}
						onUserSalaries={setUserSalaries}
					/>
					{isWarningDialogOpen && (
						<ConversationWarning
							handleCloseAllPopUps={handleCloseAllPopUps}
							handleToggleDialog={handleToggleDialog}
						/>
					)}
				</>
			)}
		</FormikWithBackup>
	);
}
