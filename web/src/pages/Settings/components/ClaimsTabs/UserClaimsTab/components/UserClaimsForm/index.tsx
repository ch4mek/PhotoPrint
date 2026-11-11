import { useTheme } from '@emotion/react';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { memo, useEffect, useMemo, useState } from 'react';

import { PermissionItem } from '../PermissionItem';
import { PERMISSIONS_POPUP_TITLE } from '../UserClaimsGrid/constants';

import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { PopUpTitle } from '@/components/PopUpTitle';
import { getEditingPopUpTitle } from '@/constants/activityStatus';
import { FormWithSaveExit } from '@/forms/components/FormWithSaveExit';
import { areArraysEqualByPrimitiveItems } from '@/helpers/areValuesEqual';
import { useResize } from '@/hooks/useResize';
import { useGetUserClaimsById } from '@/queries/useGetUserClaimsById';

import { ClaimsWrapper, UserClaimInfo, UserNameTitle } from './styles';
import { UserClaimsFormProps } from './types';
import { getClaimPermissions, getPopUpStylesConfig } from './utils';

function UserClaimsFormInner({
	handleClose,
	groupClaims,
	onSubmit,
	user,
	isLoading,
}: UserClaimsFormProps) {
	const theme = useTheme();
	const [, windowHeight] = useResize();
	const { userId, userFullName } = user;

	const { data } = useGetUserClaimsById(userId);
	const [activePermissions, setActivePermissions] = useState<string[]>([]);
	const [isDirty, setIsDirty] = useState(false);

	useEffect(() => {
		if (data) {
			const permissions = getClaimPermissions(data);
			setActivePermissions(permissions);
		}
	}, [data]);

	useEffect(() => {
		if (data) {
			const arePermissionsChanged = !areArraysEqualByPrimitiveItems(
				getClaimPermissions(data),
				activePermissions
			);
			setIsDirty(arePermissionsChanged);
		}
	}, [data, activePermissions]);

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	const handleSubmit = () => {
		onSubmit({ userId, permissions: activePermissions });
	};

	return (
		<FormWithSaveExit isFormDirty={isDirty} onClose={handleClose}>
			{(handleOutsideClose) => (
				<PopUp
					title={
						<PopUpTitle
							headerText={getEditingPopUpTitle(PERMISSIONS_POPUP_TITLE)}
							extraContent={
								<UserClaimInfo>
									<Tooltip
										text={userFullName}
										useIfTextCut
										fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
									>
										<UserNameTitle variant="m-400">
											({userFullName})
										</UserNameTitle>
									</Tooltip>
								</UserClaimInfo>
							}
						/>
					}
					onClose={handleClose}
					onShadowClick={handleOutsideClose}
					type="form"
					stylesConfig={popUpStylesConfig}
					controls={{
						positiveControl: {
							onClick: handleSubmit,
							disabled: isLoading,
						},
						negativeControl: { onClick: handleClose },
					}}
					showLoader={isLoading}
					hasPaddings
				>
					<ClaimsWrapper countColumn={groupClaims?.length}>
						{groupClaims?.map((claim) => (
							<PermissionItem
								key={claim.name}
								item={claim}
								permissions={activePermissions}
								setActivePermissions={setActivePermissions}
							/>
						))}
					</ClaimsWrapper>
				</PopUp>
			)}
		</FormWithSaveExit>
	);
}

export const UserClaimsForm = memo(UserClaimsFormInner);
