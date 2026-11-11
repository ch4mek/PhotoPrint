import {
	createContext,
	PropsWithChildren,
	useContext,
	useMemo,
	useState,
} from 'react';

import {
	PopUpData,
	ToolbarContextValue,
	ToolbarState,
} from '@/context/ToolbarContext/types';
import { EditorToolbarForm } from '@/forms/EditorToolbarForm';
import { useShowModal } from '@/pages/hooks/useShowModal';

const ToolbarContext = createContext({} as ToolbarContextValue);

export function ToolbarProvider({ children }: PropsWithChildren) {
	const [popUpData, setPopUpData] = useState<PopUpData | null>(null);
	const [isToolbarPopoverOpen, setIsToolbarPopoverOpen] =
		useState<ToolbarState>({
			isOpen: false,
			type: null,
		});

	const { handleOpen, handleClose, isOpen } = useShowModal();

	const handleSetPopUpData = (data: PopUpData) => {
		setPopUpData(data);
	};

	const handlePopUpClose = () => {
		setPopUpData(null);
		handleClose();
	};

	const handlePopoverOpen = (type: string) => {
		setIsToolbarPopoverOpen((prevState) => ({
			type,
			isOpen: !prevState.isOpen,
		}));
	};

	const handlePopoverClose = () =>
		setIsToolbarPopoverOpen({
			isOpen: false,
			type: null,
		});

	const handlePopUpOpen = () => {
		handleOpen();
		handlePopoverClose();
	};

	const toolbarContextValue: ToolbarContextValue = useMemo(
		() => ({
			setPopUpData: handleSetPopUpData,
			handleOpen: handlePopUpOpen,
			handleClose: handlePopUpClose,
			isOpen,
			popUpData,
			handlePopoverOpen,
			handlePopoverClose,
			isToolbarPopoverOpen,
		}),
		[isOpen, popUpData, isToolbarPopoverOpen]
	);

	return (
		<ToolbarContext.Provider value={toolbarContextValue}>
			{children}
			{isOpen && popUpData && (
				<EditorToolbarForm
					type={popUpData.type}
					handleClose={handleClose}
					handleSubmit={popUpData.handleSubmit}
				/>
			)}
		</ToolbarContext.Provider>
	);
}

export const useToolbarContext = () => useContext(ToolbarContext);
