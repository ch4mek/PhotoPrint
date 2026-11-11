import { useTheme } from '@emotion/react';
import { DotIcon } from '@mms/mms-ui-library/icons/base/DotIcon';

import { DraggableControlIcon } from '@/components/DraggableControlIcon';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { StaticContentWrapper } from '@/pages/Wiki/components/GroupSection/components/StaticItem/styles';
import { StaticItemProps } from '@/pages/Wiki/components/GroupSection/components/StaticItem/types';
import { ItemWrapper } from '@/pages/Wiki/components/GroupSection/styles';
import { getIdFromUniqueGroupsIds } from '@/pages/Wiki/components/GroupSection/utils';
import { UserClaims } from '@/types/claims';

export function StaticItem({
	onClick,
	isSelected,
	menuOptions,
	isMoveMode,
	item,
	disabled,
}: StaticItemProps) {
	const theme = useTheme();

	const { claims } = useAuth();

	const isShowControl = checkIsHasClaims([UserClaims.ManageArticle], claims);

	return (
		<ItemWrapper onClick={onClick} disabled={disabled}>
			<DotIcon color={theme.palette['graphics-light-gray']} />
			<StaticContentWrapper isSelected={isSelected}>
				{item.name}
			</StaticContentWrapper>
			{(isMoveMode || isShowControl) && (
				<DraggableControlIcon
					menuOptions={menuOptions}
					isMoveMode={isMoveMode}
					item={{
						id: getIdFromUniqueGroupsIds(item.id),
						name: item.name,
					}}
				/>
			)}
		</ItemWrapper>
	);
}
