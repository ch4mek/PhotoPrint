import { ChevronRightIcon } from '@mms/mms-ui-library/icons/base/ChevronRightIcon';

import { DraggableControlIcon } from '@/components/DraggableControlIcon';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { CollapseItemIcon } from '@/pages/Wiki/components/GroupSection/components/CollapseItem/styles';
import { CollapseItemProps } from '@/pages/Wiki/components/GroupSection/components/CollapseItem/types';
import {
	ItemContentWrapper,
	ItemWrapper,
} from '@/pages/Wiki/components/GroupSection/styles';
import { getIdFromUniqueGroupsIds } from '@/pages/Wiki/components/GroupSection/utils';
import { UserClaims } from '@/types/claims';
import { SegmentTypes } from '@/types/wiki';

const { Group, SubGroup } = SegmentTypes;

export function CollapseItem({
	id,
	onClick,
	title,
	expanded,
	disabled,
	getMenuItems,
	parentGroupId,
	isMoveMode,
}: CollapseItemProps) {
	const { claims } = useAuth();

	const segmentType = parentGroupId == null ? Group : SubGroup;
	const menuOptions = getMenuItems(segmentType);

	const isShowControl =
		segmentType === Group
			? checkIsHasClaims(
					[
						UserClaims.ManageGroup,
						UserClaims.ManageArticle,
						UserClaims.ManageSubgroup,
					],
					claims
			  )
			: checkIsHasClaims(
					[UserClaims.ManageSubgroup, UserClaims.ManageArticle],
					claims
			  );

	return (
		<ItemWrapper disabled={disabled} onClick={onClick}>
			<CollapseItemIcon expanded={expanded}>
				<ChevronRightIcon />
			</CollapseItemIcon>
			<ItemContentWrapper>{title}</ItemContentWrapper>
			{(isMoveMode || isShowControl) && (
				<DraggableControlIcon
					menuOptions={menuOptions}
					isMoveMode={isMoveMode}
					item={{
						id: getIdFromUniqueGroupsIds(id),
						name: title,
					}}
				/>
			)}
		</ItemWrapper>
	);
}
