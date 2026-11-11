import { Button } from '@mms/mms-ui-library/components/Button';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { SearchIcon } from '@mms/mms-ui-library/icons/base/SearchIcon';
import { memo } from 'react';
import { useTranslation } from 'react-i18next';

import { Square } from '@/components/Square';
import { COLORS, DESK_STATUS_LABEL_KEYS } from '@/constants/booking';
import { IconSize } from '@/constants/iconSizes';
import { useBookingContext } from '@/context/Booking';
import { useBookingMapContext } from '@/pages/Booking/context/BookingMapContext';
import { LocationTypeId } from '@/types/configuration';

import {
	LegendWrapper,
	ColorTooltipTextWrapper,
	ControlsWrapper,
} from './styled';

function WorkplaceControlsInner({ showLegend }: { showLegend: boolean }) {
	const { setIsMapOpen } = useBookingMapContext();
	const { office } = useBookingContext();
	const { t } = useTranslation('booking');

	const handleMapOpen = () => {
		setIsMapOpen(true);
	};

	const showButton = office.location === LocationTypeId.Minsk;

	return (
		<ControlsWrapper hasLegend={showLegend}>
			{showLegend && (
				<LegendWrapper>
					{COLORS.map((color) => {
						const labelKey = DESK_STATUS_LABEL_KEYS[color];

						return (
							<Tooltip
								key={color}
								text={
									<ColorTooltipTextWrapper>
										{t(labelKey)}
									</ColorTooltipTextWrapper>
								}
							>
								<Square type={color} side={IconSize.M} />
							</Tooltip>
						);
					})}
				</LegendWrapper>
			)}
			{showButton && (
				<Button
					view="light"
					addon={<SearchIcon />}
					addonPlacement="right"
					isFullWidth={false}
					onClick={handleMapOpen}
				>
					{t('actions.viewMap')}
				</Button>
			)}
		</ControlsWrapper>
	);
}

export const WorkplaceControls = memo(WorkplaceControlsInner);
