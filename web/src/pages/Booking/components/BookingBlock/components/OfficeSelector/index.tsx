import { DropTab } from '@mms/mms-ui-library/components/DropTab';
import { Popover } from '@mms/mms-ui-library/components/Popover';
import { useOutsideClickMany } from '@mms/mms-ui-library/hooks/useOutsideClickMany';
import { ChevronIcon } from '@mms/mms-ui-library/icons/base/ChevronIcon';
import { RefObject, useMemo, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { bookingOfficeNames } from '@/constants/booking';
import { useBookingContext, useConfiguration } from '@/context/index';
import { sortOfficeItems } from '@/helpers/sortOfficeItems';
import { OfficeSelectorOption } from '@/types/booking';
import { OfficeType } from '@/types/configuration';

import { BOOKING_DROP_TAB_WIDTH } from './constants';
import {
	OfficeName,
	OfficeSelectorChevronWrapper,
	OfficeSelectorWrapper,
} from './styled';

export function OfficeSelector() {
	const {
		configurableValues: { offices },
	} = useConfiguration();
	const { office, initialOffice, setOffice } = useBookingContext();
	const [isOpen, setIsOpen] = useState(false);
	const { t } = useTranslation('booking');

	const modalRef = useRef<HTMLDivElement>(null);
	const containerRef = useRef<HTMLDivElement>(null);

	const handleToggleIsOpen = () => {
		setIsOpen((prevOpen) => !prevOpen);
	};

	const handleClickOutside = () => {
		setIsOpen(false);
	};

	const changeOfficeLocation = (option: OfficeSelectorOption) => {
		const foundOffice = offices?.find(
			(off) => off.location === option.officeLocation
		);

		if (foundOffice) {
			setOffice(sortOfficeItems(foundOffice));
		}
		setIsOpen(false);
	};

	useOutsideClickMany(
		[modalRef, containerRef as RefObject<HTMLElement>],
		handleClickOutside
	);

	const officeNameToRender = t(bookingOfficeNames[office.location]);

	const value: OfficeType = office ?? initialOffice;

	const officeSelectorOptions = useMemo(
		() =>
			offices?.map((officeItem) => ({
				name: t(bookingOfficeNames[officeItem.location]),
				officeLocation: officeItem.location,
			})),
		[offices, t]
	);

	const valueForDroptab: OfficeSelectorOption = {
		name: t(bookingOfficeNames[value.location]),
		officeLocation: value.location,
	};

	return (
		<OfficeSelectorWrapper ref={containerRef} onClick={handleToggleIsOpen}>
			<OfficeName variant="small">{officeNameToRender}</OfficeName>
			<OfficeSelectorChevronWrapper isOpen={isOpen}>
				<ChevronIcon />
			</OfficeSelectorChevronWrapper>
			<Popover
				withTransition={false}
				ref={modalRef}
				anchorElement={containerRef.current}
				distanceToAnchorElement={9}
				open={isOpen}
				position="bottom-end"
			>
				<DropTab
					options={officeSelectorOptions}
					value={valueForDroptab}
					onChange={changeOfficeLocation}
					valueField="name"
					width={`${BOOKING_DROP_TAB_WIDTH}px`}
				/>
			</Popover>
		</OfficeSelectorWrapper>
	);
}
