import { useTheme } from '@emotion/react';
import {
	createContext,
	useMemo,
	useContext,
	useState,
	PropsWithChildren,
	useEffect,
	useCallback,
} from 'react';

import { MapPopUp } from '../components/MapPopUp';

import {
	BookingMapContextValues,
	BookingMapImagesLoadStatus,
} from '@/context/types';

const BookingMapContext = createContext({} as BookingMapContextValues);

export function BookingMapProvider({ children }: PropsWithChildren) {
	const [isMapOpen, setIsMapOpen] = useState(false);
	const [isLoading, setIsLoading] = useState(true);
	const [imagesLoadStatus, setImagesLoadStatus] =
		useState<BookingMapImagesLoadStatus>({});

	const { mode } = useTheme();

	useEffect(() => {
		setIsLoading(true);
	}, [mode]);

	const setImageLoaded = useCallback((key: string) => {
		setImagesLoadStatus((prev) => ({
			...prev,
			[key]: true,
		}));
	}, []);

	const resetImagesLoadStatus = useCallback(() => {
		setImagesLoadStatus({});
	}, []);

	const isImageLoaded = useCallback(
		(key: string): boolean => imagesLoadStatus[key] || false,
		[imagesLoadStatus]
	);

	const value: BookingMapContextValues = useMemo(
		() => ({
			isMapOpen,
			setIsMapOpen,
			isLoading,
			setIsLoading,
			imagesLoadStatus,
			resetImagesLoadStatus,
			setImageLoaded,
			isImageLoaded,
		}),
		[
			isMapOpen,
			isLoading,
			imagesLoadStatus,
			resetImagesLoadStatus,
			setImageLoaded,
			isImageLoaded,
		]
	);

	return (
		<BookingMapContext.Provider value={value}>
			{children}
			{isMapOpen && <MapPopUp />}
		</BookingMapContext.Provider>
	);
}

export const useBookingMapContext = () => useContext(BookingMapContext);
