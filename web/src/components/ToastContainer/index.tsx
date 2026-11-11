import { ToastPosition } from 'react-toastify';

import { useColorMode } from '@/context/Theme';

import { MAX_TOAST_AMOUNT, Slide, sxProps } from './constants';
import { ToastContainerWrapper } from './styles';

export function ToastContainer({ position }: { position: ToastPosition }) {
	const { theme } = useColorMode();

	// use "styles" instead "theme" attr because ToastContainer has its own "theme"
	return (
		<ToastContainerWrapper
			style={sxProps}
			position={position}
			icon={false}
			transition={Slide}
			hideProgressBar
			closeOnClick
			draggable
			limit={MAX_TOAST_AMOUNT}
			styles={theme}
		/>
	);
}
