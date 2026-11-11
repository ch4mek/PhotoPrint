import { DropIndicatorWrapper } from '@/pages/Wiki/components/GroupSection/components/DraggableItem/components/DropIndicator/styles';
import { DropIndicatorPositionType } from '@/pages/Wiki/types';

export function DropIndicator({
	position,
}: {
	position: DropIndicatorPositionType;
}) {
	return <DropIndicatorWrapper position={position} />;
}
