import { Text, View } from "react-native";

type WelcomeProps = {
    name:string,
    age:number,
    gender:boolean
}


const Welcome = (props:WelcomeProps) => {


    return (
        <View>
            <Text>Hellow {props.name}</Text>
            <Text>You are {props.age} years old</Text>
            <Text>and you are {props.gender ? "Male" : "Female"}</Text>
        </View>

    )
}

export default Welcome;