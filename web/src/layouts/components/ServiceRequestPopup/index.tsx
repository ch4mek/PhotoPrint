import { Popover } from '@mms/mms-ui-library/components/Popover';
import { useOutsideClickMany } from '@mms/mms-ui-library/hooks/useOutsideClickMany';
import { RequestsIcon } from '@mms/mms-ui-library/icons/base/RequestsIcon';
import { useCallback, useMemo, useRef } from 'react';

import { DocumentIcon } from './components/DocumentIcon';
import { SERVICE_REQUEST_BUTTON_TEXT, MENU_OPTIONS_OFFSET } from './constants';
import { MenuButton, MenuItem, MenuWrapper } from './styles';

interface ServiceRequestPopupProps {
	isOpen: boolean;
	onOpen: VoidFunction;
	onClose: VoidFunction;
	onRequestAbsence: VoidFunction;
	onRequestDocuments: VoidFunction;
}

export function ServiceRequestPopup({
	isOpen,
	onOpen,
	onClose,
	onRequestAbsence,
	onRequestDocuments,
}: ServiceRequestPopupProps) {
	const buttonRef = useRef<HTMLButtonElement>(null);
	const popoverRef = useRef(null);

	const handleClickOutside = useCallback(() => {
		if (isOpen) {
			onClose();
		}
	}, [isOpen, onClose]);

	useOutsideClickMany([buttonRef, popoverRef], handleClickOutside);

	const handleButtonClick = useCallback(() => {
		if (isOpen) {
			onClose();
		} else {
			onOpen();
		}
	}, [isOpen, onOpen, onClose]);

	const handleRequestAbsence = useCallback(() => {
		onRequestAbsence();
	}, [onRequestAbsence]);

	const handleRequestDocuments = useCallback(() => {
		onRequestDocuments();
	}, [onRequestDocuments]);

	const menuOptions = useMemo(
		() => [
			{
				icon: RequestsIcon,
				text: 'Absence',
				onClick: handleRequestAbsence,
			},
			{
				icon: DocumentIcon,
				text: 'Documents',
				onClick: handleRequestDocuments,
			},
		],
		[handleRequestAbsence, handleRequestDocuments]
	);

	return (
		<>
			<MenuButton
				ref={buttonRef}
				color="secondary"
				isFullWidth={false}
				onClick={handleButtonClick}
			>
				{SERVICE_REQUEST_BUTTON_TEXT}
			</MenuButton>
			{isOpen && (
				<Popover
					open={isOpen}
					anchorElement={buttonRef.current}
					position="bottom-end"
					offset={MENU_OPTIONS_OFFSET}
					preventOverflow={false}
				>
					<MenuWrapper ref={popoverRef}>
						{menuOptions.map(({ text, icon: Icon, onClick }) => (
							<MenuItem onClick={onClick} key={text}>
								<Icon />
								{text}
							</MenuItem>
						))}
					</MenuWrapper>
				</Popover>
			)}
		</>
	);
}
