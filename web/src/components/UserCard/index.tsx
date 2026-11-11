import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { useMemo } from 'react';
import { createSearchParams } from 'react-router-dom';

import {
	InformationTypography,
	UserCardAvatar,
	UserCardAvatarLink,
	UserCardInformation,
	UserCardLink,
	UserCardWrapper,
} from '@/components/UserCard/styles';
import { PM_ROLE } from '@/constants/absence';
import { IconSize } from '@/constants/iconSizes';
import { Routes } from '@/constants/routes';
import { PopUpSearchParams } from '@/constants/searchParams';
import { useConfiguration } from '@/context/Configuration';
import { getFullName } from '@/helpers/utils';
import { useAvatar } from '@/hooks/useAvatar';
import { AllRolesOptions } from '@/types/absences';

import { UserCardProps } from './types';

export function UserCard({
	id,
	firstName,
	lastName,
	fullName: fullNameFromProps,
	avatarFieldId,
	color,
	position,
	department,
	className,
	ref,
	isProfileLinkEnabled = false,
}: UserCardProps) {
	const theme = useTheme();
	const {
		configurableValues: { roleTypes },
	} = useConfiguration();

	const { imgSource } = useAvatar(avatarFieldId);

	const fullName = fullNameFromProps ?? getFullName(firstName, lastName);
	const role =
		position === AllRolesOptions.PM
			? PM_ROLE
			: roleTypes.find(({ name }) => name === position)?.description ?? null;

	const showProfileLink = isProfileLinkEnabled && id != null;

	const link = useMemo(
		() =>
			showProfileLink
				? {
						to: {
							pathname: Routes.home,
							search: createSearchParams({
								[PopUpSearchParams.UserIdParam]: String(id),
							}).toString(),
						},
						target: '_blank',
						rel: 'noopener noreferrer',
				  }
				: null,
		[id, showProfileLink]
	);

	const avatar = useMemo(
		() => (
			<UserCardAvatar
				size={IconSize.XL}
				src={imgSource}
				bgColor={color ?? undefined}
				{...{ firstName, lastName }}
			/>
		),
		[color, firstName, lastName, imgSource]
	);

	return (
		<UserCardWrapper {...{ className, ref }}>
			{link ? (
				<UserCardAvatarLink {...link}>{avatar}</UserCardAvatarLink>
			) : (
				avatar
			)}

			<UserCardInformation avatarWidth={IconSize.XL}>
				<Tooltip
					text={fullName}
					textCutOrientation="horizontal"
					position="bottom"
					useIfTextCut
				>
					<InformationTypography variant="m-600">
						{link ? (
							<UserCardLink {...link}>{fullName}</UserCardLink>
						) : (
							fullName
						)}
					</InformationTypography>
				</Tooltip>
				<Tooltip
					useIfTextCut
					text={role || department}
					textCutOrientation="horizontal"
					position="bottom"
				>
					<InformationTypography
						variant="m-400"
						color={theme.palette['text-gray']}
					>
						{role || department}
					</InformationTypography>
				</Tooltip>
			</UserCardInformation>
		</UserCardWrapper>
	);
}
