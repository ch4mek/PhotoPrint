import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';

import {
	LeaderboardAvatar,
	LeaderboardCardWrapper,
	LeaderboardDepartmentText,
	LeaderboardPlace,
	LeaderboardScore,
	LeaderboardUserDetails,
	LeaderboardUserInformation,
	LeaderboardUserText,
} from '../../styles';
import { LeaderboardCardProps } from '../../types';

import { IconSize } from '@/constants/iconSizes';
import { getFullName } from '@/helpers/utils';
import { useAvatar } from '@/hooks/useAvatar';

export function LeaderboardCard({ place, result }: LeaderboardCardProps) {
	const { avatarId, departmentName, firstName, lastName, score } = result;
	const { imgSource } = useAvatar(avatarId);
	const fullName = getFullName(firstName, lastName);
	const isTopResult = place <= 3;

	return (
		<LeaderboardCardWrapper {...{ isTopResult }}>
			<LeaderboardPlace variant="m-700" {...{ isTopResult }}>
				{place}
			</LeaderboardPlace>
			<LeaderboardAvatar
				size={IconSize.XL}
				src={imgSource}
				{...{ firstName, lastName }}
			/>
			<LeaderboardUserInformation>
				<Tooltip
					useIfTextCut
					text={fullName}
					textCutOrientation="horizontal"
					position="bottom"
				>
					<LeaderboardUserText variant="m-600">{fullName}</LeaderboardUserText>
				</Tooltip>
				<LeaderboardUserDetails>
					<LeaderboardDepartmentText variant="s-400">
						{departmentName || '—'}
					</LeaderboardDepartmentText>
					<LeaderboardScore variant="s-400">{score}</LeaderboardScore>
				</LeaderboardUserDetails>
			</LeaderboardUserInformation>
		</LeaderboardCardWrapper>
	);
}
