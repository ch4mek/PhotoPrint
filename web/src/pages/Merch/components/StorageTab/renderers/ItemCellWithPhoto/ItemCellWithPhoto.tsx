import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { MerchStorageGridRow } from '../../types';
import { getCellColorForRow } from '../../utils/getCellColorForRow';

import { ChevronButton } from '@/components/ChevronButton';
import { useConfiguration } from '@/context/Configuration';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { PhotoItem } from '@/pages/Merch/components/ElementsTab/components/NameWithPhotoItem/NameWithPhotoItem';
import { PopupWithFullImage } from '@/pages/Merch/components/ElementsTab/components/PopupWithFullImage/PopupWithFullImage';
import {
	FIRST_ROWS_HEIGHT,
	getGridPhotoStyleConfig,
} from '@/pages/Merch/components/ElementsTab/constants';
import { getNewCellHeight } from '@/pages/Merch/components/ElementsTab/utils/getNewCellHeight';
import { MerchStatuses } from '@/pages/Merch/constants';
import { ThemeMode } from '@/theme';

import {
	ItemCellWithPhotoContentWrapper,
	NameCellWrapper,
	NameFieldsWrapper,
	NameGeneralFields,
	NameGeneralFieldWrapper,
	NameTypography,
} from './styles';

export function ItemCellWithPhoto({
	row,
	onRowChange,
}: RenderCellProps<MerchStorageGridRow>) {
	const { handleClose, handleOpen, isOpen } = useShowModal();
	const theme = useTheme();

	const {
		configurableValues: { merchSizes },
	} = useConfiguration();

	const {
		expanded,
		name,
		photoFileId,
		isClothing,
		description,
		storage,
		sizeAmounts,
		status,
	} = row;

	const isChevronShown =
		(isClothing || sizeAmounts.length > 1) && Boolean(sizeAmounts.length);

	const handleOpenFullPhotoPopup = () => {
		handleOpen();
	};

	const handleExpandedCell = () => {
		onRowChange({
			...row,
			expanded: !expanded,
			itemsRowHeight: !expanded
				? getNewCellHeight(storage, merchSizes, isClothing)
				: FIRST_ROWS_HEIGHT,
		});
	};

	const chevronColor =
		status !== MerchStatuses.available &&
		!expanded &&
		theme.mode !== ThemeMode.LIGHT
			? theme.palette['graphics-primary-2']
			: '';

	const photoItemStyleConfig = getGridPhotoStyleConfig(theme);

	const cellColor = useMemo(
		() => getCellColorForRow(sizeAmounts, theme, expanded),
		[row, theme]
	);

	return (
		<NameCellWrapper data-expanded={expanded} color={cellColor}>
			<NameFieldsWrapper>
				<NameGeneralFields>
					<NameGeneralFieldWrapper isFirstRow>
						<ItemCellWithPhotoContentWrapper>
							<PhotoItem
								onClick={handleOpenFullPhotoPopup}
								imageId={photoFileId}
								styleConfig={photoItemStyleConfig}
								isResized
							/>
							<Tooltip useIfTextCut text={name}>
								<NameTypography variant="m-400">{name}</NameTypography>
							</Tooltip>
						</ItemCellWithPhotoContentWrapper>
						{isChevronShown && (
							<ChevronButton
								chevronColor={chevronColor}
								onClick={handleExpandedCell}
								isActive={expanded}
							/>
						)}
					</NameGeneralFieldWrapper>
				</NameGeneralFields>
			</NameFieldsWrapper>
			{isOpen && (
				<PopupWithFullImage
					handleClose={handleClose}
					description={description}
					name={name}
					photoFileId={photoFileId}
				/>
			)}
		</NameCellWrapper>
	);
}
