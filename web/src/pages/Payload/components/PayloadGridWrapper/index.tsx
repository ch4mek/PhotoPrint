import { useEffect } from 'react';

import { usePayloadGridHover } from '../../hooks/usePayloadGridHover';

import { PopUpSearchParams } from '@/constants/searchParams';
import { usePayloadContext } from '@/context/PayloadContext';
import { PayloadForm } from '@/forms/PayloadForm';
import { useSearchParamsUrl } from '@/hooks/useSearchParamsUrl';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { PayloadGrid } from '@/pages/Payload/components/PayloadGrid';

import { PayloadGridColumnHoverWrapper, Wrapper } from './styles';

export function PayloadGridWrapper() {
	const { id: modalId, handleOpen, handleClose, isOpen } = useShowModal();
	const { targetRef, handleMouseLeave, handleMouseOver } =
		usePayloadGridHover<HTMLDivElement>();
	const { searchParams } = useSearchParamsUrl();
	const { payloadGridRef, scrollPositionRef } = usePayloadContext();

	useEffect(() => {
		const userId = searchParams.get(PopUpSearchParams.UserIdParam);

		if (userId && payloadGridRef.current) {
			scrollPositionRef.current = payloadGridRef.current.scrollTop;
		}
	}, [searchParams]);

	return (
		<Wrapper>
			<PayloadGridColumnHoverWrapper
				onMouseOver={handleMouseOver}
				onMouseLeave={handleMouseLeave}
				ref={targetRef}
			>
				<PayloadGrid handleOpen={handleOpen} />
			</PayloadGridColumnHoverWrapper>
			{isOpen && (
				<PayloadForm handleClose={handleClose} id={modalId as number} />
			)}
		</Wrapper>
	);
}
