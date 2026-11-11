import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';

import {
	InformationTypography,
	UserCardAvatar,
	UserCardInformation,
	UserCardWrapper,
} from '../UserCard/styles';

import { COMMENT_TIMESTAMP } from '@/constants/date';
import { IconSize } from '@/constants/iconSizes';
import { formatDateString } from '@/helpers/dateToISOString';
import { getFullName } from '@/helpers/formatName';
import { useAvatar } from '@/hooks/useAvatar';

import { TimeInformationTypography } from './styled';
import { UserCommentCardProps } from './types';

export function UserCommentCard({
	createdByUser,
	createdAtUtc,
	timeSuffix,
}: UserCommentCardProps) {
	const theme = useTheme();
	const { avatarFieldId, color, firstName, lastName } = createdByUser;

	const { imgSource } = useAvatar(avatarFieldId);

	const fullName = getFullName(firstName, lastName);

	return (
		<UserCardWrapper>
			<UserCardAvatar
				size={IconSize.XL}
				src={imgSource}
				bgColor={color ?? undefined}
				{...{ firstName, lastName }}
			/>

			<UserCardInformation avatarWidth={IconSize.XL}>
				<Tooltip
					useIfTextCut
					text={fullName}
					textCutOrientation="vertical"
					position="bottom"
				>
					<InformationTypography variant="m-600">
						{fullName}
					</InformationTypography>
				</Tooltip>
				<TimeInformationTypography
					variant="m-400"
					color={theme.palette.text.secondary}
				>
					{formatDateString(createdAtUtc, '-', COMMENT_TIMESTAMP)}
					{timeSuffix}
				</TimeInformationTypography>
			</UserCardInformation>
		</UserCardWrapper>
	);
}
