import { useTheme } from '@emotion/react';
import { DownloadButton } from '@mms/mms-ui-library/components/DownloadButton';
import { Popover } from '@mms/mms-ui-library/components/Popover';
import { useOnClickOutside } from '@mms/mms-ui-library/hooks/useOnClickOutside';
import { useRef, useState } from 'react';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { zIndexes } from '@/constants/zIndexes';

import { ExportModal } from './components/ExportModal';
import { DISTANCE_FROM_BUTTON_TO_POPOVER } from './constants';
import { DownloadButtonWrapper } from './styles';
import { ExportControlProps } from './types';

export function ExportControl<T>({
	onChange,
	options,
	onPerUserExportToggle,
	isPerUserExport,
	isDisabled = false,
	modalWidth = 'auto',
}: ExportControlProps<T>) {
	const [isModalShown, setIsModalShown] = useState(false);
	const popoverRef = useRef<HTMLDivElement>(null);
	const downloadButtonRef = useRef<HTMLDivElement>(null);
	const theme = useTheme();

	const closePopover = () => {
		setIsModalShown(false);
	};

	useOnClickOutside([downloadButtonRef, popoverRef], closePopover);

	const toggleOpenPopover = () => {
		setIsModalShown((prevIsModalShown) => !prevIsModalShown);
	};

	return (
		<>
			<DownloadButtonWrapper ref={downloadButtonRef}>
				<DownloadButton disabled={isDisabled} onClick={toggleOpenPopover} />
			</DownloadButtonWrapper>
			<Popover
				zIndex={zIndexes.filterPopup}
				ref={popoverRef}
				open={isModalShown}
				anchorElement={downloadButtonRef.current}
				distanceToAnchorElement={DISTANCE_FROM_BUTTON_TO_POPOVER}
				position="bottom-end"
				wrapperStyles={{
					background: theme.palette['background-white'],
					borderRadius: BorderRadiuses.m,
					width: modalWidth,
					border: `1px solid ${theme.palette['graphics-input-stroke']}`,
					boxShadow: theme.namedShadows.dropDownListWrapper,
				}}
			>
				<ExportModal
					{...{
						onPerUserExportToggle,
						onChange,
						options,
						isPerUserExport,
						closePopover,
					}}
				/>
			</Popover>
		</>
	);
}
