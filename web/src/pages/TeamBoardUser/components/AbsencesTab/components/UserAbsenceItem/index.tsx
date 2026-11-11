import {
	PickerButton,
	PickerButtonOption,
} from '@mms/mms-ui-library/components/PickerButton';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { RequestIcon } from '@mms/mms-ui-library/icons/base';
import { OptionDotsIcon } from '@mms/mms-ui-library/icons/base/OptionDotsIcon';
import { SettingsIcon } from '@mms/mms-ui-library/icons/base/SettingsIcon';
import { UserCrossIcon } from '@mms/mms-ui-library/icons/base/UserCrossIcon';
import { useCallback, useMemo, useRef } from 'react';
import { useTranslation } from 'react-i18next';

import { DeleteConfirmationDialog } from '@/components/DeleteConfirmationDialog';
import { UserAbsenceCard } from '@/components/UserAbsenceCard';
import { DateFormat } from '@/constants/date';
import { isChiefRole } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { AbsenceSettingsForm } from '@/forms/Absence/AbsenceSettingsForm';
import { checkIsCurrentUserId } from '@/helpers/checkIsCurrentUserId';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { getDateFormat } from '@/helpers/getDateFormat';
import { useAddScrollListener } from '@/hooks/useAddScrollListener';
import dayjs from '@/lib/dayjs';
import { TeamboardUserAccessLevel } from '@/pages/TeamBoardUser/types';
import {
	isLimitedCoordinatorAccess,
	isLimitedAccountantAccess,
} from '@/pages/TeamBoardUser/utils';
import { UserClaims } from '@/types/claims';

import {
	controlsText,
	REMOVE_ASSIGN_POPUP_TITLE,
	REMOVE_ASSIGN_CONFIRMATION_MESSAGE,
} from './constants';
import { useAbsenceSettings } from './hooks/useAbsenceSettings';
import { useRemoveAssign } from './hooks/useRemoveAssign';
import { UserAbsenceItemProps } from './types';

export function UserAbsenceItem({
	absenceItem,
	handleOpen,
	userId,
	accessLevel = TeamboardUserAccessLevel.Full,
}: UserAbsenceItemProps) {
	const { role, claims, id } = useAuth();
	const { t } = useTranslation('common', { keyPrefix: 'controlButtons' });

	const pickerButtonRef = useRef<HTMLButtonElement>(null);
	const [isOpenPopover, setIsOpenPopover] =
		useAddScrollListener(pickerButtonRef);

	const {
		isAgreement,
		agreementTemplateId,
		inquiryTemplateId,
		dateFrom,
		type,
	} = absenceItem;

	const templateId = inquiryTemplateId ?? agreementTemplateId;

	const {
		handleOpen: handleRemoveAssignOpen,
		handleClose: handleRemoveAssignClose,
		isOpen: isRemoveAssignOpen,
		handleFormSubmit: handleRemoveAssignFormSubmit,
		isLoading: isRemoveAssignFormLoading,
	} = useRemoveAssign();

	const {
		handleAbsenceSettingsFormOpen,
		handleAbsenceSettingsFormClose,
		isAbsenceSettingsFormOpen,
		isAbsenceSettingsLoading,
		handleAbsenceSettingsSubmit,
	} = useAbsenceSettings();

	const handleRequestClick = useCallback(() => {
		handleOpen({ templateId, formType: type });
	}, [handleOpen, templateId, type]);

	const handleRemoveAssignSubmit = useCallback(() => {
		handleRemoveAssignFormSubmit({
			assigneUserIds: [],
			unassigneUserIds: [Number(userId)],
			inquiryTemplateId,
			agreementId: agreementTemplateId ?? null,
			effectiveFrom: isAgreement
				? getDateFormat(dayjs(), DateFormat.YYYY_MM_DD)
				: null,
		});
		handleRemoveAssignClose();
	}, [
		handleRemoveAssignFormSubmit,
		handleRemoveAssignClose,
		absenceItem,
		userId,
		dateFrom,
	]);

	const isCurrentUser = useMemo(
		() => checkIsCurrentUserId(id, userId),
		[id, userId]
	);

	const menuOptions = useMemo<Array<PickerButtonOption>>(() => {
		const canManageSettings =
			!isCurrentUser &&
			(isChiefRole(role) ||
				checkIsHasClaims([UserClaims.ManageHRModule], claims));

		const hasPermissionsToRemoveAssign =
			isChiefRole(role) ||
			checkIsHasClaims(
				[UserClaims.ManageAbsenceSettings, UserClaims.ManageHRModule],
				claims
			);

		const canRemoveAssign = !isCurrentUser && hasPermissionsToRemoveAssign;

		const shouldHideRequest =
			isLimitedCoordinatorAccess(accessLevel) ||
			isLimitedAccountantAccess(accessLevel);

		const settingOption =
			canManageSettings && agreementTemplateId
				? [
						{
							addon: <SettingsIcon />,
							onClick: () => handleAbsenceSettingsFormOpen(agreementTemplateId),
							value: controlsText.settings,
						},
				  ]
				: [];

		const removeAssignOption = canRemoveAssign
			? [
					{
						addon: <UserCrossIcon />,
						onClick: () => handleRemoveAssignOpen(inquiryTemplateId),
						value: controlsText.removeAssign,
					},
			  ]
			: [];

		const requestOption = shouldHideRequest
			? []
			: [
					{
						addon: <RequestIcon />,
						onClick: () => handleRequestClick(),
						value: t('request'),
					},
			  ];

		return [...requestOption, ...settingOption, ...removeAssignOption];
	}, [
		role,
		claims,
		isCurrentUser,
		agreementTemplateId,
		inquiryTemplateId,
		handleAbsenceSettingsFormOpen,
		handleRemoveAssignOpen,
	]);

	const actions = menuOptions.length > 0 && (
		<PickerButton
			item={absenceItem}
			icon={<OptionDotsIcon />}
			options={menuOptions}
			ref={pickerButtonRef}
			{...{ isOpenPopover, setIsOpenPopover }}
		/>
	);

	return (
		<>
			<UserAbsenceCard absenceItem={absenceItem} actions={actions} />

			{isRemoveAssignOpen && (
				<DeleteConfirmationDialog
					title={REMOVE_ASSIGN_POPUP_TITLE}
					customTitle
					content={
						<Typography variant="m-400">
							{REMOVE_ASSIGN_CONFIRMATION_MESSAGE}
						</Typography>
					}
					showConfirmationMessage={false}
					isLoading={isRemoveAssignFormLoading}
					handleClose={handleRemoveAssignClose}
					handleSubmit={handleRemoveAssignSubmit}
				/>
			)}

			{isAbsenceSettingsFormOpen && (
				<AbsenceSettingsForm
					handleClose={handleAbsenceSettingsFormClose}
					onSubmit={handleAbsenceSettingsSubmit}
					isLoading={isAbsenceSettingsLoading}
					userId={userId!}
					formType={absenceItem.type}
					absenceName={absenceItem.name}
				/>
			)}
		</>
	);
}
