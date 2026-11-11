import { LeftHexagonWrapper, RightHexagonWrapper } from './styles';
import { useHexagonIcons } from './useHexagonIcons';

export function Hexagons() {
	const [LeftHexagonIcon, RightHexagonIcon] = useHexagonIcons();

	return (
		<>
			<LeftHexagonWrapper>
				<LeftHexagonIcon />
			</LeftHexagonWrapper>
			<RightHexagonWrapper>
				<RightHexagonIcon />
			</RightHexagonWrapper>
		</>
	);
}
