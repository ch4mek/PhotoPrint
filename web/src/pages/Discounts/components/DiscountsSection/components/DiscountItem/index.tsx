import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { BasketIcon } from '@mms/mms-ui-library/icons/base/BasketIcon';
import { EditIcon } from '@mms/mms-ui-library/icons/base/EditIcon';
import { useMemo } from 'react';

import { DraggableControlIcon } from '@/components/DraggableControlIcon';
import { MenuItem } from '@/components/DraggableControlIcon/types';
import { ViewImage } from '@/components/ViewImage';
import { zIndexes } from '@/constants/zIndexes';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { formatDiscountPercent } from '@/helpers/formatDiscountPercent';
import { UserClaims } from '@/types/claims';

import {
	ControlIconWrapper,
	ControlItemRow,
	DiscountBadge,
	DiscountItemWrapper,
	TitleTypographyWrapper,
} from './styles';
import { DiscountCardProps } from './types';

export function DiscountItem({
	item,
	handleDeleteClick,
	handleEditClick,
	handleItemClick,
	isMoveMode,
}: DiscountCardProps) {
	const { claims } = useAuth();

	const handleDiscountItemClick = () => {
		handleItemClick(item.id);
	};

	const itemMenuOption = useMemo(
		() => [
			{
				value: 'Edit',
				addon: <EditIcon />,
				onClick: ({ id }: MenuItem) => handleEditClick(id),
			},
			{
				value: 'Delete',
				addon: <BasketIcon />,
				onClick: ({ id, name }: MenuItem) => handleDeleteClick(id, name),
			},
		],
		[]
	);

	const formatPercentValue = useMemo(
		() => formatDiscountPercent(item.discountPercent, item.discountPercentTo),
		[item]
	);

	return (
		<DiscountItemWrapper onClick={handleDiscountItemClick}>
			{itemMenuOption && (
				<ControlIconWrapper>
					<DraggableControlIcon
						iconSize="large"
						menuOptions={itemMenuOption}
						isMoveMode={isMoveMode}
						withIconSpace={false}
						isDisablePickerButton={checkIsHasClaims(
							[UserClaims.ManageDiscountCard],
							claims
						)}
						item={{
							id: item.id,
							name: item.name,
						}}
					/>
				</ControlIconWrapper>
			)}
			<ViewImage imageId={item.imageFileId} title={item.name} />
			<ControlItemRow>
				<Tooltip
					zIndex={zIndexes.gridTooltip}
					text={item.name}
					position="bottom"
					fallbackPlacements={['bottom']}
					useIfTextCut
				>
					<TitleTypographyWrapper>{item.name}</TitleTypographyWrapper>
				</Tooltip>
				<DiscountBadge>{formatPercentValue}</DiscountBadge>
			</ControlItemRow>
		</DiscountItemWrapper>
	);
}
