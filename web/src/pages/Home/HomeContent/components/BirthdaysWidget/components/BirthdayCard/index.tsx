import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';

import { IconSize } from '@/constants/iconSizes';
import { getFullName } from '@/helpers/utils';
import { useAvatar } from '@/hooks/useAvatar';

import {
	BirthdayAvatar,
	BirthdayCardInformation,
	BirthdayCardWrapper,
	InformationTypography,
} from './styles';
import { BirthdayCardProps } from './types';

export function BirthdayCard({ item, backgroundImage }: BirthdayCardProps) {
	const { department, avatarFileId, lastName, firstName } = item;

	const { imgSource } = useAvatar(avatarFileId);

	return (
		<BirthdayCardWrapper {...{ backgroundImage }}>
			<BirthdayAvatar
				size={IconSize.XL}
				src={imgSource}
				{...{ firstName, lastName }}
			/>
			<BirthdayCardInformation>
				<Tooltip
					useIfTextCut
					text={getFullName(firstName, lastName)}
					textCutOrientation="vertical"
					position="bottom"
				>
					<InformationTypography variant="m-700" align="center">
						{getFullName(firstName, lastName)}
					</InformationTypography>
				</Tooltip>
				<InformationTypography variant="s-400" align="center">
					{department}
				</InformationTypography>
			</BirthdayCardInformation>
		</BirthdayCardWrapper>
	);
}
